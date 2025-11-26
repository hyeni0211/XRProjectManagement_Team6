using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VAvatar;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VMap;
using TwentyOz.VivenSDK.Scripts.Editor.Manifest;
using Twoz.Viven.Common.Setting;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Profile;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    // TODO: SDK 빌드 시 ScriptableObject들 초기화하는 로직이 필요함. 테스트용 설정이 SDK에 포함됨
    // TODO: MapBuildSetting, AddressablesSetting의 remote, build path, ObjectBuildSetting 등을 초기화한 후 패키지 빌드 

    /// <summary>
    /// VivenSDK에서 사용하는 빌드 매니저 클래스입니다.
    /// VMap, VObject, VAvatar를 빌드하고 관리하는 기능을 제공합니다.
    /// </summary>
    public static class VivenBuildManager
    {
    #region FIELDS

        /// <summary>
        /// VivenLauncher가 TestOnViven 모드에서 VMap을 불러올 임시 경로
        /// </summary>
        private static string TestOnVivenPath => $"{Path.GetTempPath()}/VivenSDK/";

        private const string ManifestFileName = "manifest.json";
        private const string ManifestHashFileName = "manifest.hash";
        private const string MetadataFileName = "metadata.json";
        private const string MetadataHashFileName = "metadata.hash";


    #region Platform Change Event

        /// <summary>
        /// Viven Build시 Platform이 변경되었을때 실행되는 Event
        /// </summary>
        public static event Action<VivenPlatform> PlatformChanged;

    #endregion

    #region Build Event

        public static Func<bool> OnBuildVMapCondition;
        public static Action OnBuildVMapStart;
        public static Action OnBuildVMapEnd;

        /// <summary>
        /// VMap Build 이벤트를 초기화합니다.
        /// </summary>
        public static void ResetBuildVMapAction()
        {
            OnBuildVMapCondition = null;
            OnBuildVMapStart = null;
            OnBuildVMapEnd = null;
        }

        public static Func<bool> OnBuildVObjectCondition;
        public static Action OnBuildVObjectStart;
        public static Action OnBuildVObjectEnd;

        /// <summary>
        /// VMap Build 이벤트를 초기화합니다.
        /// </summary>
        public static void ResetBuildVObjectAction()
        {
            OnBuildVObjectCondition = null;
            OnBuildVObjectStart = null;
            OnBuildVObjectEnd = null;
        }


        public static Func<bool> OnBuildVAvatarCondition;
        public static Action OnBuildVAvatarStart;
        public static Action OnBuildVAvatarEnd;

        /// <summary>
        /// VMap Build 이벤트를 초기화합니다.
        /// </summary>
        public static void ResetBuildVAvatarAction()
        {
            OnBuildVAvatarCondition = null;
            OnBuildVAvatarStart = null;
            OnBuildVAvatarEnd = null;
        }

    #endregion

    #endregion

    #region Build Interface

        public static BuildResultData TryBuildBundle(VivenBuildType buildType, VivenBuildData buildData,
            VivenContentBuildProfiles buildProfiles)
        {
            // 이전 빌드 프로필을 저장
            var prevProfile = BuildProfile.GetActiveBuildProfile();
            BuildTarget previousBuildTarget = default;
            if (!prevProfile)
            {
                previousBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            }

            var result = TryBuildBundle_Impl(buildType, buildData, buildProfiles);

            // 빌드 후 원래의 빌드 프로필로 복구
            if (prevProfile)
            {
                BuildProfile.SetActiveBuildProfile(prevProfile);
            }
            else if (previousBuildTarget != default)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(previousBuildTarget),
                    previousBuildTarget);
            }

            return result;
        }

        public static BuildResultData ManualBuildVMap(VivenBuildType buildType, VivenBuildData buildData,
            VivenContentBuildProfiles buildProfiles, VivenPlatform platform)
        {
            // 빌드 결과물이 저장될 경로를 확인합니다.
            var buildDirectory = Path.GetDirectoryName(VivenAddressableSetting.RemoteBuildPlatformPath);
            
            var stopwatch = Stopwatch.StartNew();
            // 빌드 진행
            var buildProfile = buildProfiles[platform];
            var platformBuildResult =
                BuildContentForPlatform(buildType, buildData, platform, buildProfile, buildDirectory);
            stopwatch.Stop();
            var buildDuration = stopwatch.ElapsedMilliseconds / 1000.0f;
            
            if (platformBuildResult.IsSuccess)
            {
                Debug.Log($"Check {buildDirectory}");
            }

            return new BuildResultData(
                platformBuildResult.BuildTarget,
                platformBuildResult.BuildMessage,
                platformBuildResult.IsSuccess,
                buildDuration,
                platformBuildResult.OutputPath);
        }

        public static BuildResultData ManualBuildBundle(VivenBuildType buildType, VivenBuildData buildData)
        {
            // 빌드 경로 확인
            var tempDirectory = Path.GetDirectoryName(VivenAddressableSetting.RemoteBuildPlatformPath);

            if (tempDirectory == null)
                return BuildResultData.Fail(buildData, "빌드 경로가 null입니다.");

            if (!Directory.Exists(tempDirectory))
            {
                Debug.LogError($"빌드 경로가 존재하지 않습니다: {tempDirectory}");
                return BuildResultData.Fail(buildData, "빌드 경로가 존재하지 않습니다.");
            }

            // "win, mac, aos, ios" 폴더 등 플랫폼 빌드 결과를 제외한 모든 폴더 및 파일을 삭제합니다.
            // 빌드 중 오류 등으로 임시 파일이 제거되지 않았을 경우를 대비함.
            foreach (var file in Directory.GetFiles(tempDirectory))
            {
                File.Delete(file);
            }

            foreach (var directory in Directory.GetDirectories(tempDirectory))
            {
                if (Path.GetFileName(directory).StartsWith("win", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetFileName(directory).StartsWith("mac", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetFileName(directory).StartsWith("aos", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetFileName(directory).StartsWith("ios", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Directory.Delete(directory, true);
            }

            // 메타데이터 파일 생성
            GenerateMetadataJson(tempDirectory, buildData);

            // 플랫폼 별 빌드가 완료되면 전체 폴더를 압축합니다.
            var saveToPath = EditorUtility.SaveFolderPanel($"Save {buildType.GetGroupName()} to Folder", "", "");
            ArchiveFolderImpl(tempDirectory, buildType.GetExtension(), saveToPath, buildData.GetBuildName());

            return BuildResultData.Success(buildData, "Build Success");
        }

    #endregion

    #region Build Method

        private static BuildResultData TryBuildBundle_Impl(VivenBuildType buildType, VivenBuildData buildData,
            VivenContentBuildProfiles buildProfiles)
        {
            var stopwatch = Stopwatch.StartNew();
            float elapsedSeconds = 0f;
            var tempPath = FileUtil.GetUniqueTempPathInProject();
            var buildPlatforms = new List<VivenPlatform>();

            try
            {
                if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
                Directory.CreateDirectory(tempPath);

                ClearBuildDirectory(); // 기존 ServerData/Build 폴더 정리

                foreach (var platform in VivenPlatformExtension.Platforms)
                {
                    if (!buildData.GetPlatformSceneWrapper(platform).enabled)
                        continue;

                    var buildProfile = buildProfiles[platform];
                    var platformBuildResult =
                        BuildContentForPlatform(buildType, buildData, platform, buildProfile, tempPath);

                    if (!platformBuildResult.IsSuccess)
                    {
                        return platformBuildResult; // 실패 시 즉시 반환
                    }

                    buildPlatforms.Add(platform);
                }

                if (!buildPlatforms.Any())
                {
                    stopwatch.Stop();
                    elapsedSeconds = stopwatch.ElapsedMilliseconds/1000.0f;
                    return BuildResultData.Fail(buildData, "활성화된 빌드 플랫폼이 없습니다.", elapsedSeconds);
                }

                // 메타데이터 파일을 작성합니다.
                GenerateMetadataJson(tempPath, buildData);
                
                // 빌드 시간을 계산합니다.
                stopwatch.Stop();
                elapsedSeconds = stopwatch.ElapsedMilliseconds/1000.0f;

                // 플랫폼 별 빌드가 완료되면 전체 폴더를 압축합니다.
                var saveToPath = EditorUtility.SaveFolderPanel($"Save {buildType.GetGroupName()} to Folder", "", "");
                if (string.IsNullOrEmpty(saveToPath))
                {
                    return BuildResultData.Fail(buildData, "저장 경로가 선택되지 않았습니다.", elapsedSeconds);
                }

                ArchiveFolderImpl(tempPath, buildType.GetExtension(), saveToPath, buildData.GetBuildName());

                Debug.Log("Build Success in " + elapsedSeconds + " seconds");
                return BuildResultData.Success(buildData, "Build Success", elapsedSeconds,
                    Path.Combine(saveToPath, $"{buildData.GetBuildName()}.{buildType.GetExtension()}"));
            }
            catch (Exception ex)
            {
                Debug.LogError($"TryBuildBundle 중 예외 발생: {ex}");
                return BuildResultData.Fail(buildData, $"빌드 중 오류 발생: {ex.Message}");
            }
        }

        private static BuildResultData BuildContentForPlatform(VivenBuildType buildType, VivenBuildData buildData,
            VivenPlatform platform, BuildProfile buildProfile, string baseTempPath)
        {
            ChangeActiveBuildProfile(buildProfile, platform);

            // 플랫폼 별 빌드 경로 생성
            var platformSpecificTempPath = Path.Combine(baseTempPath, platform.GetPlatformDirectory());
            // 플랫폼 폴더가 이미 존재한다면 해당 폴더를 삭제합니다.
            if (Directory.Exists(platformSpecificTempPath))
            {
                // 일반적으로 이전에 ClearBuildDirectory() 또는 tempPath 전체 삭제로 인해 이 경우는 드물지만, 안전을 위해 삭제
                Directory.Delete(platformSpecificTempPath, true);
            }

            // 빌드 가능 여부 확인
            var validateResult = buildType switch
            {
                VivenBuildType.vmap => VivenBuildValidator.CanBuildMap( /*(buildData as VivenMapBuildData)?.mapName*/),
                VivenBuildType.vobject => VivenBuildValidator.CanBuildObject((buildData as VivenObjectBuildData)
                    ?.gameObject),
                VivenBuildType.vavatar => VivenBuildValidator.CanBuildAvatar((buildData as VivenAvatarBuildData)
                    ?.targetAvatar),
                _ => throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null)
            };

            if (!validateResult.IsSuccess)
            {
                Debug.LogError("Build Check Failed : " + buildProfile.name); // 실패 시 로그 출력
                return validateResult;
            }

            // 빌드 조건 확인
            var conditionMet = buildType switch
            {
                VivenBuildType.vmap => OnBuildVMapCondition?.Invoke() ?? true,
                VivenBuildType.vobject => OnBuildVObjectCondition?.Invoke() ?? true,
                VivenBuildType.vavatar => OnBuildVAvatarCondition?.Invoke() ?? true,
                _ => throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null)
            };

            if (!conditionMet)
            {
                Debug.LogError("Build Condition Failed : " + buildProfile.name); // 실패 시 로그 출력
                return BuildResultData.Fail(buildData, "빌드 조건이 충족되지 않습니다.");
            }

            var buildResult = buildType switch
            {
                VivenBuildType.vmap => BuildVMap((VivenMapBuildData)buildData, platform),
                VivenBuildType.vobject => BuildVObject((VivenObjectBuildData)buildData, platform),
                VivenBuildType.vavatar => BuildVAvatar((VivenAvatarBuildData)buildData, platform),
                _ => throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null)
            };

            if (!buildResult.IsSuccess)
            {
                Debug.LogError("Build Failed : " + buildProfile.name); // 실패 시 로그 출력
                return buildResult;
            }

            var sourceBuiltPath = VivenAddressableSetting.RemoteBuildPlatformPath; // Addressable 빌드 결과 경로
            if (!Directory.Exists(sourceBuiltPath))
            {
                Debug.LogError($"{platform} Build Failed: 빌드된 콘텐츠 폴더({sourceBuiltPath})를 찾을 수 없습니다.");
                return BuildResultData.Fail(buildData, "빌드된 콘텐츠 폴더를 찾을 수 없습니다.");
            }

            try
            {
                Debug.Log($"{platform} 콘텐츠 이동 시도 {sourceBuiltPath} -> {platformSpecificTempPath}");

                // 대소문자 변경을 위해 2번 이동
                // IOS 플랫폼 빌드 결과물은 .../iOS 폴더에 생성됨
                // VivenSDK 결과물은 .../ios 폴더에 생성되어야 하기 때문에 폴더명을 변경해야 함
                // .../iOS -> .../ios 는 동일한 폴더로 인식하기 때문에 Move가 동작하지 않음. 
                // 따라서 임시 폴더로 이동 후 다시 이동함
                var tmpPath = sourceBuiltPath + "_TEMP";
                if (Directory.Exists(tmpPath))
                {
                    Directory.Delete(tmpPath, true);
                }
                
                Directory.Move(sourceBuiltPath, tmpPath);
                Directory.Move(tmpPath, platformSpecificTempPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"{platform} 콘텐츠 이동 실패 {sourceBuiltPath} -> {platformSpecificTempPath}: {e.Message}");
                return BuildResultData.Fail(buildData, $"빌드된 콘텐츠 이동 중 오류 발생: {e.Message}");
            }

            return buildResult;
        }

        /// <summary>
        /// VObject을 AddressableAsset으로 빌드합니다.
        /// </summary>
        private static BuildResultData BuildVObject(VivenObjectBuildData objBuildData, VivenPlatform platform)
        {
            OnBuildVObjectStart?.Invoke();
            var result = Build_Impl(objBuildData, platform);
            OnBuildVObjectEnd?.Invoke();

            return result;
        }

        /// <summary>
        /// VivenMap을 빌드합니다.
        /// <br/>
        /// 빌드 Pack에 따라 vivenAdditionalProperty를 가져와서 추가합니다.
        /// </summary>
        /// <param name="mapBuildData"></param>
        /// <param name="platform"></param>
        private static BuildResultData BuildVMap(VivenMapBuildData mapBuildData, VivenPlatform platform)
        {
            if (!PrepareBuild(mapBuildData, platform, out var buildPrepareResult))
            {
                return buildPrepareResult;
            }

            OnBuildVMapStart?.Invoke();
            var result = Build_Impl(mapBuildData, platform);
            OnBuildVMapEnd?.Invoke();

            return result;
        }

        /// <summary>
        /// VObject을 AddressableAsset으로 빌드합니다.
        /// </summary>
        private static BuildResultData BuildVAvatar(VivenAvatarBuildData avatarBuildData, VivenPlatform platform)
        {
            OnBuildVAvatarStart?.Invoke();
            var result = Build_Impl(avatarBuildData, platform);
            OnBuildVAvatarEnd?.Invoke();

            return result;
        }

        /// <summary>
        /// VMap, VObject, VAvatar을 AddressableAsset으로 빌드합니다.
        /// </summary>
        /// <param name="buildData"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        private static BuildResultData Build_Impl(VivenBuildData buildData, VivenPlatform platform)
        {
            // 1. Addressable 설정을 초기화
            var setting =
                VivenAddressableSetting.InitializeAddressableSetting(buildData, platform, out var initSettingResult);
            if (!initSettingResult.IsSuccess)
            {
                Debug.LogError($"Addressable 설정 초기화 실패: {initSettingResult}");
                return initSettingResult;
            }

            // 2. 추가 콘텐츠 빌드 (Map Objects 등)
            var additionalBuildResult = buildData.BuildAdditionalObjects(setting);
            if (!additionalBuildResult.IsSuccess)
            {
                Debug.LogError($"MapObject 빌드 실패: {additionalBuildResult}");
                return additionalBuildResult;
            }

            // 3. Addressable 컨텐츠 빌드
            // Addressable 빌드 캐시 클리어
            // AddressableAssetSettings.CleanPlayerContent();
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings
                .ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();
            // 빌드 폴더가 유효한 지 확인합니다.
            var contentBuildResult = VivenBuildValidator.ValidateAddressableBuildResult();
            if (!contentBuildResult.IsSuccess)
            {
                Debug.LogError("빌드 결과가 유효하지 않습니다.");
                return contentBuildResult;
            }

            // 4. manifest.json 파일 및 hash 파일 생성
            if (!GenerateManifestAndHashFiles(VivenAddressableSetting.RemoteBuildPlatformPath,
                buildData.GetContentProperties()))
            {
                Debug.LogError("Manifest 파일 또는 Hash 파일 생성에 실패했습니다.");
                return BuildResultData.Fail(buildData, "Manifest 파일 또는 Hash 파일 생성에 실패했습니다.");
            }

            return BuildResultData.Success(buildData, "Build Success");
        }

        /// <summary>
        /// 빌드 전 VivenMapEnvironment를 설정합니다.
        /// </summary>
        /// <param name="mapBuildData"></param>
        /// <param name="platform"></param>
        /// <param name="mapEnvResult"></param>
        /// <returns></returns>
        private static bool PrepareBuild(VivenMapBuildData mapBuildData, VivenPlatform platform,
            out BuildResultData mapEnvResult)
        {
            // 1. 빌드할 Scene을 가져옵니다.
            var scenePath = mapBuildData.GetPlatformSceneWrapper(platform).targetPath;
            if (string.IsNullOrEmpty(scenePath) || !File.Exists(scenePath))
            {
                Debug.LogError($"VMap 빌드 오류: 유효하지 않은 Scene 경로입니다. Path: {scenePath}");
                mapEnvResult = BuildResultData.Fail(mapBuildData, "유효하지 않은 Scene 경로입니다.");
                return false;
            }

            // NOTICE: 빌드 전에 Scene을 열어야 정상적으로 맵을 빌드할 수 있음.
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 2. Scene에서 VivenMapEnvironment를 찾습니다.
            var roots = scene.GetRootGameObjects();
            var vivenMapEnvironment = roots.FindAnyComponentInChildren<VivenMapEnvironment>(true);

            // VivenMapEnvironment가 없으면 에러를 발생시킵니다.
            if (!vivenMapEnvironment)
            {
                Debug.LogError("VivenMapEnvironment를 찾을 수 없습니다.");
                mapEnvResult = BuildResultData.Fail(mapBuildData, "VivenMapEnvironment가 없습니다.");
                return false;
            }

            // 현재 ProjectSettings/Quality에서 설정한 RenderPipelineAsset을 가져와 RenderPipelineSetting을 생성합니다.
            vivenMapEnvironment.renderPipelineSetting = VivenRenderPipelineSetting.GetRenderPipelineSetting();

            // 변경 사항을 저장합니다.
            EditorUtility.SetDirty(vivenMapEnvironment);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            mapBuildData.SetMapProperties(vivenMapEnvironment);

            mapEnvResult = BuildResultData.Success("", "MapEnvironment Setting Success");
            return true;
        }

        public static BuildResultData BuildMapObjects(VivenMapBuildData mapBuildData, AddressableAssetSettings settings)
        {
            var mapObjects = mapBuildData.mapObjects;

            // 1. MapObject를 추가할 AddressableGroup을 찾습니다.
            var mapBuildGroup = settings.groups
                .FirstOrDefault(g => g.Name == mapBuildData.BuildType.GetGroupName());
            if (!mapBuildGroup)
            {
                return BuildResultData.Fail(mapBuildData, "Addressable Group을 찾을 수 없습니다.");
            }

            // 2. MapObject 검증
            
            // MapObject의 Prefab이 유효한지 확인합니다.
            var invalidMapObjects = mapObjects
                .Where(mapObject => !VivenBuildValidator.Validate(mapObject))
                .ToList();
            
            if (invalidMapObjects.Any())
            {
                foreach (var mapObject in invalidMapObjects)
                {
                    Debug.LogError($"MapObject {mapObject.Key}를 빌드할 수 없습니다.");
                }
                
                return BuildResultData.Fail(mapBuildData, "MapObject 빌드에 실패했습니다.");
            }

            // 3. Addressable Group에 MapObject를 추가합니다.
            foreach (var mapObject in mapObjects)
            {
                var targetPath = AssetDatabase.GetAssetPath(mapObject.Prefab);
                // object를 Addressable Group에 추가
                var sceneEntry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(targetPath), mapBuildGroup);

                // 주소는 mapObject의 Key로 설정
                sceneEntry.address = mapObject.Key;
            }

            // 4. Addressable Asset 설정 변경 사항 저장
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            return BuildResultData.Success(mapBuildData, "MapObject Build Success");
        }

        private static void GenerateMetadataJson(string path, VivenBuildData buildData)
        {
            var metadata = buildData switch
            {
                VivenMapBuildData data => VivenBuildDataWriter.WriteMapBuildData(data),
                VivenObjectBuildData data => VivenBuildDataWriter.WriteVObjectBuildData(data),
                VivenAvatarBuildData data => VivenBuildDataWriter.WriteVAvatarBuildData(data),
                _ => string.Empty
            };
            var metadataPath = Path.Combine(path, MetadataFileName); // 상수 사용
            File.WriteAllText(metadataPath, metadata);

            var hashPath = Path.Combine(path, MetadataHashFileName); // 상수 사용
            File.WriteAllText(hashPath, metadata.GetHashCode().ToString()); // metadata 내용의 해시를 사용
        }

        private static bool GenerateManifestAndHashFiles(string basePath, VivenContentProperty[] contentProperties)
        {
            try
            {
                var manifestPath = Path.Combine(basePath, ManifestFileName);
                var manifest = new VivenSDKManifestData();

                var manifestJson = manifest.ToJson(contentProperties);
                File.WriteAllText(manifestPath, manifestJson);

                var hashPath = Path.Combine(basePath, ManifestHashFileName);
                // manifestJson 내용의 해시를 사용해야 파일 내용 변경 시 해시값도 변경됩니다.
                File.WriteAllText(hashPath, manifestJson.GetHashCode().ToString());
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to generate manifest or hash files: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 빌드 프로필을 변경하고 플랫폼 변경 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="buildProfile">활성화할 빌드 프로필</param>
        /// <param name="platform">변경할 타겟 플랫폼</param>
        private static void ChangeActiveBuildProfile(BuildProfile buildProfile, VivenPlatform platform)
        {
            // 빌드 플랫폼 변경
            BuildProfile.SetActiveBuildProfile(buildProfile);

            // 빌드 프로필 변경 후 컴파일이 완료될 때까지 대기
            // 참고: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Build.Profile.BuildProfile.SetActiveBuildProfile.html
            // "(전략)... All script files will be compiled on the next Editor update."
            AssetDatabase.Refresh();

            // 플랫폼 변경 Event 실행
            PlatformChanged?.Invoke(platform);
        }

    #endregion

    #region ARCHIVE FILES

        /// <summary>
        /// 선택한 컨텐츠를 압축하여 저장합니다.
        /// </summary>
        private static void ArchiveFolderImpl(string sourceFolderPath, string targetExtension, string destinationPath,
            string destinationName)
        {
            // archive files to zip
            var zipFilePath = sourceFolderPath + $".{targetExtension}";
            if (File.Exists(zipFilePath)) File.Delete(zipFilePath);

            // SourcePath를 Zip으로 압축
            ZipFile.CreateFromDirectory(sourceFolderPath, zipFilePath);

            // Delete Directory of SourcePath
            if (Directory.Exists(sourceFolderPath))
                Directory.Delete(sourceFolderPath, true);

            var saveFilePath = Path.Join(destinationPath, $"{destinationName}.{targetExtension}");

            //파일이 존재하면 삭제
            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);

            // zip 파일을 save 위치로 이동
            File.Move(zipFilePath, saveFilePath);
        }

    #endregion

    #region Test On Viven

        /// <summary>
        /// VivenMap을 에디터에서 실행하기 위한 임시 기능
        /// </summary>
        public static BuildResultData BuildVMapOnLocalTemp()
        {
            // VivenClient 가 "Path.GetTempPath()/VivenSDK/" 에서 VMap을 실행하기 때문에 경로 유지해줘야 함
            var tmpPath = TestOnVivenPath;

            // Clear all files in Tmp Directory
            if (Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true); // Delete Directory
            Directory.CreateDirectory(tmpPath); // Create Directory

            if (!VivenBuildValidator.CanBuildMap().IsSuccess)
                return BuildResultData.Fail("TestOnViven", "Local Build Failed");

            var testMapBuildData = VivenTestMapBuildData.Instance;
            testMapBuildData.Init();

            var setting =
                VivenAddressableSetting.InitializeAddressableSetting(testMapBuildData, VivenPlatform.WIN,
                    out var initSettingResult);

            if (!initSettingResult.IsSuccess)
            {
                Debug.LogError($"TestOnViven VMap 빌드 실패: {initSettingResult}");
                return initSettingResult;
            }

            // MapObject를 빌드합니다.
            var additionalBuildResult = testMapBuildData.BuildAdditionalObjects(setting);
            if (!additionalBuildResult.IsSuccess)
            {
                Debug.LogError($"MapObject 빌드 실패: {additionalBuildResult}");
                return additionalBuildResult;
            }

            // REVIEW: Local Path를 Remote로 변경해서 멀티 테스트를 지원?

            // TestOnViven 용 Remote Build&Load Path로 변경
            setting.profileSettings.SetValue(setting.activeProfileId,
                VivenAddressableSetting.AddressableConstants.RemoteBuildPath, tmpPath);
            setting.profileSettings.SetValue(setting.activeProfileId,
                VivenAddressableSetting.AddressableConstants.RemoteLoadPath, tmpPath);

            setting.OverridePlayerVersion = "0.1";

            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings
                .ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();

            // Catalog.json 파일이 생성되었는 지 검증함.
            var bundlePath = VivenAddressableSetting.RemoteBuildPlatformPath;
            if (!File.Exists(bundlePath + $"/catalog_{setting.OverridePlayerVersion}.json"))
            {
                Debug.LogError("Catalog.json 파일이 생성되지 않았습니다.");
                return BuildResultData.Fail("TestOnViven", "Catalog.json 파일이 생성되지 않았습니다.");
            }

            return BuildResultData.Success("TestOnViven", "Local Build Success");
        }

    #endregion

        public static bool ManualBuildFolderExists(VivenPlatform platform)
        {
            var buildDirectory = Path.GetDirectoryName(VivenAddressableSetting.RemoteBuildPlatformPath);
            var platformPath = Path.Join(buildDirectory, platform.GetPlatformDirectory());
            return Directory.Exists(platformPath);
        }

        public static void ClearManualBuildFolder(VivenPlatform platform)
        {
            var buildDirectory = Path.GetDirectoryName(VivenAddressableSetting.RemoteBuildPlatformPath);

            // 빌드 성공 시 플랫폼 경로롤 AssetBundle을 이동합니다.
            // var platformPath = tempPath + "/" + platform.GetPlatformDirectory();
            var platformPath = Path.Join(buildDirectory, platform.GetPlatformDirectory());

            if (Directory.Exists(platformPath))
            {
                Directory.Delete(platformPath, true);
            }
        }

        public static void ClearBuildDirectory()
        {
            var buildDirectory = Path.GetDirectoryName(VivenAddressableSetting.RemoteBuildPlatformPath);

            if (string.IsNullOrEmpty(buildDirectory))
                return;

            if (Directory.Exists(buildDirectory))
            {
                Directory.Delete(buildDirectory!, true);
            }
        }

        /// <summary>
        /// 플랫폼 정보를 json으로 저장합니다.
        /// </summary>
        /// <param name="buildType"></param>
        /// <param name="tempPath"></param>
        /// <param name="buildPlatforms"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [Obsolete]
        private static void GeneratePlatformJson(VivenBuildType buildType, string tempPath,
            List<VivenPlatform> buildPlatforms)
        {
            var manifestPath = tempPath + "/platform.json";
            string json;
            switch (buildType)
            {
                case VivenBuildType.vmap:
                    // manifest 파일을 생성합니다.
                    var mapManifest = new VMapBundleManifestData();
                    mapManifest.AvailablePlatforms =
                        buildPlatforms.Select(platform => platform.GetPlatformDirectory()).ToList();
                    json = mapManifest.ToJson();
                    break;
                case VivenBuildType.vobject:
                    // manifest 파일을 생성합니다.
                    var objManifest = new VObjectBuildManifestData();
                    objManifest.AvailablePlatforms =
                        buildPlatforms.Select(platform => platform.GetPlatformDirectory()).ToList();
                    json = objManifest.ToJson();
                    break;
                case VivenBuildType.vavatar:
                    // manifest 파일을 생성합니다.
                    var avtManifest = new VivenAvatarBuildManifestData();
                    avtManifest.AvailablePlatforms =
                        buildPlatforms.Select(platform => platform.GetPlatformDirectory()).ToList();
                    json = avtManifest.ToJson();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null);
            }

            File.WriteAllText(manifestPath, json);
        }
    }
}