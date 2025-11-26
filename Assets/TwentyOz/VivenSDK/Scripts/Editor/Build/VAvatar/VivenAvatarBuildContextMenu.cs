using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar;
using TwentyOz.VivenSDK.Scripts.Editor.UI.Build;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VAvatar
{
    /// <summary>
    /// VAvatar Build Context 메뉴입니다.
    /// Editor에서 Prefab을 선택했을 때 Viven Avatar Build Window을 열 수 있습니다.
    /// </summary>
    public static class VivenAvatarBuildContextMenu
    {
        private static VivenBuildSetting BuildSetting => VivenBuildSetting.Global;

        /// <summary>
        /// Viven Avatar Build Window을 엽니다.
        /// </summary>
        [MenuItem("Assets/Viven/Build Viven Avatar")]
        private static void OpenVivenAvatarBuildWindow()
        {
            var selected = Selection.activeObject as GameObject;

            if (!ValidateSelection(selected)) return;

            // 빌드 가능한지 확인합니다.
            var validateResult = VivenBuildValidator.CanBuildAvatar(selected);
            if (validateResult.IsSuccess)
            {
                // VivenAvatarBuildWindow.ShowWindow(selected);
                var buildData = VivenAvatarBuildData.Get(selected);
                
                // VObject 빌드
                var result = VivenBuildManager.TryBuildBundle(VivenBuildType.vavatar, buildData, BuildSetting.contentBuildProfiles);
                
                // 빌드 결과를 표시합니다.
                BuildResultWindow.ShowWindow(result);
            }
            else
            {
                BuildResultWindow.ShowWindow(validateResult);
            }
        }

        /// <summary>
        /// Prefab을 선택했을 때만 옵션이 활성화되도록 합니다.
        /// </summary>
        /// <returns>옵션이 선택 가능한지 여부</returns>
        [MenuItem("Assets/Viven/Build Viven Avatar", true)]
        private static bool OpenVivenAvatarBuildWindowValidation()
        {
            var selected = Selection.activeObject as GameObject;
            return ValidateSelection(selected);
        }
        
        private static bool ValidateSelection(GameObject selected)
        {
            // Prefab을 선택했을 때만 옵션이 활성화되도록 합니다.
            if (selected == null || !PrefabUtility.IsPartOfPrefabAsset(selected))
            {
                return false;
            }

            // SDKVivenAvatar가 없는 경우 옵션이 활성화되지 않습니다.
            if (!selected.TryGetComponent<SDKVivenAvatar>(out _))
            {
                return false;
            }

            return true;
        }


        // /// <summary>
        // /// 콘텐츠 ID와 버전을 임의로 지정해 Viven Avatar를 빌드합니다.
        // /// </summary>
        // [MenuItem("Assets/Viven/Quick Build Viven Avatar")]
        // private static void QuickBuildVivenAvatar()
        // {
        //     var selected = Selection.activeObject as GameObject;
        //
        //     // 선택한 오브젝트가 유효한지 확인합니다.            
        //     if (!ValidateSelection(selected)) return;
        //
        //     // Viven Avatar를 빌드합니다.
        //     var buildData = ScriptableObject.CreateInstance<VivenAvatarBuildData>();
        //     buildData.targetAvatar = selected;
        //     buildData.WIN.enabled = true;
        //     buildData.WIN.targetPath = AssetDatabase.GetAssetPath(selected);
        //     var result = VivenBuildManager.TryBuildVAvatar(buildData, VivenPlatform.WIN);
        //
        //     // 빌드 결과를 표시합니다.
        //     BuildResultWindow.ShowWindow(result);
        // }
        //
        // /// <summary>
        // /// Prefab을 선택했을 때만 옵션이 활성화되도록 합니다.
        // /// </summary>
        // /// <returns>옵션이 선택 가능한지 여부</returns>
        // [MenuItem("Assets/Viven/Quick Build Viven Avatar", true)]
        // private static bool QuickBuildVivenAvatarValidation()
        // {
        //     var selected = Selection.activeObject as GameObject;
        //     return ValidateSelection(selected);
        // }
    }
}