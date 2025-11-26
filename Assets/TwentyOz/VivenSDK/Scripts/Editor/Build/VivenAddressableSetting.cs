using System;
using System.IO;
using System.Linq;
using TwentyOz.VivenSDK.Scripts.Editor.Core;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    public static class VivenAddressableSetting
    {
        /// <summary>
        /// 로컬 에서 Addressable 빌드 시 사용할 경로를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static string GetAddressableBundleBuildDirectory()
        {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            var cachePath = settings.profileSettings
                .GetValueByName(settings.activeProfileId, VivenAddressableSetting.AddressableConstants.RemoteBuildPath)
                .Replace("[BuildTarget]", EditorUserBuildSettings.activeBuildTarget.ToString());
            return cachePath;
        }
        
        public static AddressableAssetSettings InitializeAddressableSetting(VivenBuildData buildData,
            VivenPlatform platform,
            out BuildResultData buildResult)
        {
            string targetPath = buildData.GetPlatformSceneWrapper(platform).targetPath;

            // Addressable Asset 설정 가져오기
            var settings = GetDefaultAddressableBuildSetting(buildData);

            if (settings == null)
            {
                buildResult = BuildResultData.Fail(buildData, "Addressable Asset 설정을 가져오는데 실패했습니다.");
                return null;
            }

            // 새로운 Addressable Group 생성
            var newGroup = CreateVivenAddressableAssetGroup(settings, buildData.BuildType.GetGroupName());

            // Scene을 Addressable Group에 추가
            var sceneEntry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(targetPath), newGroup);

            // 주소는 VObject의 cttId + cttBinVal로 설정
            sceneEntry.address = buildData.BuildType.GetExtension();

            // AddressableSetting 변경 사항 저장
            EditorUtility.SetDirty(settings);

            // 세팅 변경 사항 저장
            AssetDatabase.SaveAssets();

            buildResult = BuildResultData.Success(buildData, buildData.GetBuildName());
            return settings;
        }

        /// <summary>
        /// 빌드 시 AddressableSetting을 초기화합니다
        /// </summary>
        private static AddressableAssetSettings GetDefaultAddressableBuildSetting(VivenBuildData buildData)
        {
            // Addressable Asset Default설정 가져오기
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            // Addressable Asset 설정 초기화
            settings.UniqueBundleIds = true;
            settings.BuiltInBundleNaming = BuiltInBundleNaming.Custom;
            settings.BuiltInBundleCustomNaming = Guid.NewGuid().ToString();
            settings.MonoScriptBundleNaming = MonoScriptBundleNaming.Custom;
            settings.MonoScriptBundleCustomNaming = settings.BuiltInBundleCustomNaming;

            //RemoteCatalog를 True로 설정
            settings.BuildRemoteCatalog = true;

            // 빌드 경로 설정
            var userId = VivenLauncher.GetUserInfo().mbrId;
            var serverDataPath = $"ServerData/{userId}/{buildData.GetBuildName()}/[BuildTarget]";
            var loadDataPath = @"{{REMOTE_PATH}}/";

            settings.profileSettings.SetValue(settings.activeProfileId, AddressableConstants.RemoteBuildPath,
                serverDataPath);
            settings.profileSettings.SetValue(settings.activeProfileId, AddressableConstants.RemoteLoadPath,
                loadDataPath);

            // Catalog의 Player Version Override를 설정
            settings.OverridePlayerVersion = "v2";

            // Addressable Asset 설정이 올바르게 되어있는지 확인합니다.
            CheckAddressableBuildSettings(settings);

            // REMOTE_BUILD_PLATFORM_PATH
            var prevPath = settings.profileSettings
                .GetValueByName(settings.activeProfileId, AddressableConstants.RemoteBuildPath)
                .Replace("[BuildTarget]", EditorUserBuildSettings.activeBuildTarget.ToString());

            // 이전 빌드 데이터가 남아있다면 삭제
            if (Directory.Exists(prevPath))
                Directory.Delete(prevPath, true);
            return settings;
        }

        private static AddressableAssetGroup CreateVivenAddressableAssetGroup(AddressableAssetSettings settings,
            string groupName)
        {
            // 기존에 존재하는 Group을 삭제함.
            var groups = settings.groups.ToList();
            foreach (var group in groups)
                settings.RemoveGroup(group);

            // Viven Asset Group 생성
            var newGroup = settings.CreateGroup(groupName, true, false, false, null);
            var schema = newGroup.AddSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(settings, VivenAddressableSetting.AddressableConstants.RemoteBuildPath);
            schema.LoadPath.SetVariableByName(settings, VivenAddressableSetting.AddressableConstants.RemoteLoadPath);
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            return newGroup;
        }

        /// <summary>
        /// Addressable Build Setting이 유효한 지 검증합니다.
        ///
        /// </summary>
        /// <remarks>
        /// AddressableAssetSetting을 확인합니다.
        /// </remarks>
        /// <param name="settings">사용중인 AddressableBuildSetting</param>
        /// <exception cref="Exception"></exception>
        private static void CheckAddressableBuildSettings(AddressableAssetSettings settings)
        {
            // JsonCatalog가 활성화되어 있는 지 확인합니다.
            if (!settings.EnableJsonCatalog)
            {
                throw new Exception("AddressableAssetSetting에서 EnableJsonCatalog를 활성화해주세요.");
            }

            // 현재 빌드 세팅에 ENABLE_JSON_CATALOG 심볼이 있는 지 확인합니다.
            if (CheckJsonCatalogSymbol() == false)
            {
                throw new Exception("현재 빌드 세팅(빌드 프로파일)에 ENABLE_JSON_CATALOG 심볼이 존재하지 않습니다.");
            }

            // Addressable Asset 설정에서 RemoteCatalog의 Build & Load Path가 Remote로 설정되어 있는 지 확인합니다.
            if (settings.RemoteCatalogBuildPath.GetName(settings) != AddressableConstants.RemoteBuildPath
                || settings.RemoteCatalogLoadPath.GetName(settings) != AddressableConstants.RemoteLoadPath)
            {
                throw new Exception("AddressableAssetSetting에서 Build & Load Path를 Remote로 설정해주세요.");
            }
        }

        /// <summary>
        /// Addressable 에셋 설정 관련 상수들을 정의하는 내부 클래스
        /// </summary>
        public static class AddressableConstants
        {
            /// <summary>
            /// 원격 빌드 경로 설정 키
            /// </summary>
            internal const string RemoteBuildPath = "Remote.BuildPath";

            /// <summary>
            /// 원격 로드 경로 설정 키
            /// </summary>
            internal const string RemoteLoadPath = "Remote.LoadPath";
        }

        /// <summary>
        /// Addressable 빌드 파일들이 저장되는 플랫폼별 경로
        /// </summary>
        public static string RemoteBuildPlatformPath => GetAddressableBundleBuildDirectory();

        public static bool CheckJsonCatalogSymbol()
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var symbols);
            if (!symbols.Contains("ENABLE_JSON_CATALOG"))
            {
                throw new Exception($"BuildTarget:{namedBuildTarget.TargetName}: ENABLE_JSON_CATALOG 심볼이 존재하지 않습니다");
            }

            return true;
        }
    }
}