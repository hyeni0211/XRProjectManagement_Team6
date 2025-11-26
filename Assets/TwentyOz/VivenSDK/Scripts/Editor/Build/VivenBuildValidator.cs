using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using TwentyOz.VivenSDK.Scripts.Core.Lua;
using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar;
using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields;
using TwentyOz.VivenSDK.Scripts.Editor.Build.Platform;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VMap;
using TwentyOz.VivenSDK.Scripts.Editor.Core;
using TwentyOz.VivenSDK.Scripts.Editor.Lua;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    public static class VivenBuildValidator
    {
        /// <summary>
        /// 콘텐츠 버전을 검증합니다. 버전이 올바르지 않다면 에디터 다이얼로그를 띄웁니다.
        /// 콘텐츠 버전은 X.Y.Z 형식으로 입력해야 합니다. (예: 0.0.1)
        /// X : 메이저 버전
        /// Y : 마이너 버전
        /// Z : 패치 버전
        /// <seealso href ="https://semver.org/"> semver 참고 </seealso>
        /// </summary>
        /// <param name="buildVersion">콘텐츠 버전</param>
        /// <returns>버전 유효 여부</returns>
        public static bool ValidateBuildVersion(string buildVersion)
        {
            // 콘텐츠 버전이 입력되지 않았을 때
            if (string.IsNullOrEmpty(buildVersion))
            {
                EditorUtility.DisplayDialog("VivenSDK", "콘텐츠 버전을 입력해주세요.", "확인");
                return false;
            }

            // check format using regex
            var semverRegex = new Regex(@"^\d+\.\d+\.\d+$");

            // if buildVersion is not "X.Y.Z" format, show dialog
            if (!semverRegex.IsMatch(buildVersion))
            {
                EditorUtility.DisplayDialog("VivenSDK", "콘텐츠 버전은 X.Y.Z 형식으로 입력해주세요.\n(예: 0.0.1)", "확인");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 콘텐츠 ID를 검증합니다. 콘텐츠 ID가 올바르지 않다면 에디터 다이얼로그를 띄웁니다.
        /// 콘텐츠 ID는 UUID 형식으로 입력해야 합니다. (예 : 3de31e8d-17af-4834-af7d-ebe37009b82b)
        /// </summary>
        /// <param name="cttId">콘텐츠 ID</param>
        /// <returns>콘텐츠 ID 유효 여부</returns>
        public static bool IsGuid(string cttId)
        {
            if (string.IsNullOrEmpty(cttId))
            {
                EditorUtility.DisplayDialog("VivenSDK", "콘텐츠 ID를 입력해주세요.", "확인");
                return false;
            }

            // if cttId is not GUID format, show dialog
            // check format using regex
            Regex guidRegex = new Regex(@"^\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$");
            if (!guidRegex.IsMatch(cttId))
            {
                EditorUtility.DisplayDialog("VivenSDK",
                    "콘텐츠 ID는 GUID 형식으로 입력해주세요.\n(예: 3de31e8d-17af-4834-af7d-ebe37009b82b)", "확인");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 모든 빌드 전에 실행되는 검증을 확인하는 함수
        /// </summary>
        /// <param name="errorMessage">검증 실패 시 에러 로그</param>
        /// <returns></returns>
        public static bool ValidateBuild(out string errorMessage)
        {
            // OpenXR 패키지 설치 확인 - VR이 필요한 플랫폼에 대해 검사
            if (!PlatformBuildConfigurator.ValidateOpenXRPackage())
            {
                errorMessage = "OpenXR Plugin 패키지가 설치되어 있지 않습니다. Package Manager에서 OpenXR Plugin 패키지를 설치하세요.";
                return false;
            }

            // 기본 Validation Check
            if (!VivenLauncher.ValidateVivenConnection())
            {
                errorMessage = "Viven 서버에 연결되어 있지 않습니다.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 현재 Scene을 VivenMap으로 빌드할 수 있는지 확인합니다.
        /// Viven서버에 로그인되어있고, Scene에 존재하하면 안되는 오브젝트가 있는 지 확인합니다.
        /// </summary>
        /// <returns>VMap 빌드 가능 여부</returns>
        public static BuildResultData CanBuildMap()
        {
            if (!ValidateBuild(out var errorLog))
            {
                EditorUtility.DisplayDialog("Error", errorLog, "OK");
                return BuildResultData.Fail("VMap", errorLog);
            }

            // Scene에 Camera가 있는지 확인
            if (Object.FindAnyObjectByType<Camera>(FindObjectsInactive.Include))
            {
                Debug.LogError("Scene에 Camera가 있을 수 없습니다.");
                EditorUtility.DisplayDialog("Error", "Scene에 Camera가 있을 수 없습니다.", "OK");
                return BuildResultData.Fail("VMap", "Scene에 Camera가 있을 수 없습니다.");
            }

            // Scene에 VivenMapEnvironment가 있는지 확인
            if (!Object.FindAnyObjectByType<VivenMapEnvironment>())
            {
                Debug.LogError("Scene에 VivenMapEnvironment가 없습니다.");
                EditorUtility.DisplayDialog("Error", "Scene에 VivenMapEnvironment가 없습니다.", "OK");
                return BuildResultData.Fail("VMap", "Scene에 VivenMapEnvironment가 없습니다.");
            }

            // Scene에 EventSystem이 있는지 확인
            if (Object.FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include))
            {
                Debug.LogError("Scene에 EventSystem을 삭제해주세요.");
                EditorUtility.DisplayDialog("Error", "Scene에 EventSystem을 삭제해주세요.", "OK");
                return BuildResultData.Fail("VMap", "Scene에 EventSystem을 삭제해주세요.");
            }

            // Scene에 SDKNetworkObject가 있는 지 확인, Build 시 ContentType이 Prepared여야 함
            var networkObjects = Object.FindObjectsByType<Twoz.Viven.Interactions.VObject>(FindObjectsSortMode.None);
            var isOk = networkObjects.Aggregate(true,
                (current, netComponent) =>
                    current & Validate(netComponent, SDKContentType.Prepared, out _));

            // Scene에 VivenLuaBehaviour가 있는지 확인
            var vivenLuaBehaviours =
                Object.FindObjectsByType<VivenLuaBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var vivenLuaBehaviour in vivenLuaBehaviours)
            {
                if (!TwozLuaChecker.Check(vivenLuaBehaviour))
                {
                    Debug.LogError("VivenLuaBehaviour에 오류가 있습니다: " + vivenLuaBehaviour.name);
                    isOk = false;
                }
            }

            if (!isOk)
            {
                EditorUtility.DisplayDialog("Error", "빌드에 실패했습니다.\n콘솔 에러 로그를 확인해주세요", "OK");
                return BuildResultData.Fail("VMap", "빌드에 실패했습니다.\n콘솔 에러 로그를 확인해주세요");
            }

            return BuildResultData.Success("VMap", "VMap 빌드 가능합니다.");
        }

        /// <summary>
        /// 선택한 프리팹을 VivenObject로 빌드할 수 있는지 확인합니다.
        /// </summary>
        /// <returns>VObject 빌드 가능 여부</returns>
        public static BuildResultData CanBuildObject(GameObject prefab)
        {
            if (!ValidateBuild(out var errorLog))
            {
                EditorUtility.DisplayDialog("Error", errorLog, "OK");
                return BuildResultData.Fail(prefab.name, errorLog);
            }

            // Prefab 내부 구성 요소를 검사합니다.
            var assetPath = AssetDatabase.GetAssetPath(prefab);
            // Load the contents of the Prefab Asset.
            var contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);

            // VObject 확인
            var networkObject = contentsRoot.GetComponentInChildren<Twoz.Viven.Interactions.VObject>();
            if (!Validate(networkObject, SDKContentType.VObject, out var errorMessage))
            {
                Debug.LogError(errorMessage);
                EditorUtility.DisplayDialog("Error", errorMessage, "OK");
                return BuildResultData.Fail(prefab.name, errorMessage);
            }

            var isOk = true;
            // Check Viven Scripts
            var vivenScripts = contentsRoot.GetComponentsInChildren<VivenLuaBehaviour>();
            foreach (var script in vivenScripts)
            {
                if (TwozLuaChecker.Check(script))
                {
                    continue;
                }

                Debug.LogError("VivenLuaBehaviour에 오류가 있습니다: " + script.name);
                isOk = false;
            }

            if (isOk == false)
            {
                return BuildResultData.Fail(prefab.name, "VivenLuaBehaviour에 오류가 있습니다.");
            }

            return BuildResultData.Success(prefab.name, "VivenObject 빌드 가능합니다.");
        }

        public static BuildResultData CanBuildAvatar(GameObject prefab)
        {
            if (!ValidateBuild(out var errorLog))
            {
                EditorUtility.DisplayDialog("Error", errorLog, "OK");
                return BuildResultData.Fail(prefab.name, errorLog);
            }

            // Prefab 내부 구성 요소를 검사합니다.
            var assetPath = AssetDatabase.GetAssetPath(prefab);

            // Load the contents of the Prefab Asset.
            var contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);

            // SDKVivenAvatar 확인
            var vavatar = contentsRoot.GetComponentInChildren<SDKVivenAvatar>();

            if (vavatar == null)
            {
                Debug.LogError("Prefab에 SDKVivenAvatar가 없습니다.");
                EditorUtility.DisplayDialog("Error", "Prefab에 SDKVivenAvatar가 없습니다.", "OK");
                return BuildResultData.Fail(prefab.name, "Prefab에 SDKVivenAvatar가 없습니다.");
            }

            // Animator 확인
            var animator = contentsRoot.GetComponentInChildren<Animator>();

            if (animator == null)
            {
                Debug.LogError("Prefab에 Animator가 없습니다.");
                EditorUtility.DisplayDialog("Error", "Prefab에 Animator가 없습니다.", "OK");
                return BuildResultData.Fail(prefab.name, "Prefab에 Animator가 없습니다.");
            }

            // CustomEmoteComponent 확인
            //

            // OutfitComponent 확인
            //

            // FacialExpressionComponent 확인
            //

            return BuildResultData.Success(prefab.name, "VivenAvatar 빌드 가능합니다.");
        }

        /// <summary>
        /// Catalog.json 파일이 생성되었는 지 검증합니다.
        /// </summary>
        /// <returns>Catalog.json 파일 존재 여부</returns>
        public static BuildResultData ValidateAddressableBuildResult()
        {
            // Catalog.json 파일이 생성되었는 지 검증함.
            if (!File.Exists(VivenAddressableSetting.RemoteBuildPlatformPath + "/catalog_v2.json"))
            {
                Debug.LogError("catalog_v2.json 파일이 생성되지 않았습니다.");
                return BuildResultData.Fail("", "catalog_v2.json 파일이 생성되지 않았습니다.");
            }

            return BuildResultData.Success("", "빌드 결과 검증 성공");
        }

    #region Component Validation

        public static bool Validate(VivenMapObject mapObject)
        {
            if (mapObject == null)
            {
                Debug.LogError("VivenMapObject가 null입니다.");
                return false;
            }

            // VivenMapObject의 Key가 null인지 확인
            if (string.IsNullOrEmpty(mapObject.Key))
            {
                Debug.LogError("VivenMapObject의 Key가 null입니다.");
                return false;
            }

            // VivenMapObject의 Prefab이 null인지 확인
            if (!mapObject.Prefab)
            {
                Debug.LogError("VivenMapObject의 Prefab이 null입니다.");
                return false;
            }

            // 4. MapObject에 VObject가 존재한다면, VObject의 contentType이 MapContent인지 확인
            var vObject = mapObject.Prefab.GetComponentInChildren<Twoz.Viven.Interactions.VObject>();
            if (vObject)
            {
                var validation = Validate(vObject, SDKContentType.MapContent, out _);
                if (!validation) return false;
            }

            return true;
        }


        public static bool Validate(Twoz.Viven.Interactions.VObject vObject, SDKContentType contentType,
            out string errorMessage)
        {
            if (vObject == null)
            {
                Debug.LogError("VObject가 null입니다.");
                errorMessage = "VObject가 null입니다.";
                return false;
            }

            // 맵에 배치된 VObject는 Prepared여야 함
            if (vObject.contentType != contentType)
            {
                Debug.LogWarning($"{vObject.name} : SDKNetworkObject의 ContentType을 {contentType}로 변경합니다.");
                vObject.contentType = contentType;

                // 변경이 실패하면 빌드 취소
                if (vObject.contentType != contentType)
                {
                    Debug.LogError($"변경을 시도했지만 SDKNetworkObject의 ContentType이 {contentType}가 아닙니다." + vObject.name);
                    errorMessage = $"변경을 시도했지만 SDKNetworkObject의 ContentType이 {contentType}가 아닙니다." + vObject.name;
                    return false;
                }

                EditorUtility.SetDirty(vObject);
            }

            // objectId가 00000000-0000-0000-0000-000000000000인 경우
            if (vObject.objectId.Equals(Guid.Empty.ToString()))
            {
                Debug.LogError("VObject의 objectId가 설정되지 않았습니다." + vObject.name);
                errorMessage = "VObject의 objectId가 설정되지 않았습니다." + vObject.name;
                return false;
            }

            AssetDatabase.SaveAssetIfDirty(vObject);

            errorMessage = string.Empty;
            return true;
        }

    #endregion
    }
}