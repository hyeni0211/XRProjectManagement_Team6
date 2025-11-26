using TwentyOz.VivenSDK.Scripts.Editor.UI.Build;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VObject
{
    /// <summary>
    /// Viven Object Context Menu입니다.
    /// Editor에서 Prefab을 선택했을 때 Viven Object Build Window을 열 수 있습니다.
    /// </summary>
    public static class VivenObjectBuildContextMenu
    {
        private static VivenBuildSetting BuildSetting => VivenBuildSetting.Global;
        
        /// <summary>
        /// Viven Object Build Window을 엽니다.
        /// </summary>
        [MenuItem("Assets/Viven/Build Viven Object")]
        private static void OpenVivenObjectBuildWindow()
        {
            // Prefab을 선택했을 때만 옵션이 활성화되도록 합니다.
            var selected = (GameObject)Selection.activeObject;
            if (selected == null || !PrefabUtility.IsPartOfPrefabAsset(selected)) return;
            // 빌드 가능한지 확인합니다.
            var validateResult = VivenBuildValidator.CanBuildObject(selected);
            if (validateResult.IsSuccess)
            {
                // VivenObjectBuildWindow.ShowWindow(selected);
                
                var buildData = VivenObjectBuildData.Get(selected);
                
                // VObject 빌드
                var result = VivenBuildManager.TryBuildBundle(VivenBuildType.vobject, buildData, BuildSetting.contentBuildProfiles);
                
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
        [MenuItem("Assets/Viven/Build Viven Object", true)]
        private static bool OpenVivenObjectBuildWindowValidation()
        {
            Object selected = Selection.activeObject;
            return selected != null && PrefabUtility.IsPartOfPrefabAsset(selected);
        }


        // /// <summary>
        // /// 콘텐츠 ID와 버전을 임의로 지정해 Viven Object를 빌드합니다.
        // /// </summary>
        // [MenuItem("Assets/Viven/Quick Build Viven Object")]
        // private static void QuickBuildVivenObject()
        // {
        //     // Prefab을 선택했을 때만 옵션이 활성화되도록 합니다.
        //     var selected = (GameObject)Selection.activeObject;
        //     // Prefab이 아닌 경우 빌드를 진행하지 않습니다.
        //     if (selected == null || !PrefabUtility.IsPartOfPrefabAsset(selected))
        //         return;
        //     // 빌드 가능한지 확인합니다.
        //     var validateResult = VivenBuildManager.ValidateCanBuildObject(selected);
        //     if (!validateResult.IsBuildSuccess)
        //     {
        //         BuildResultWindow.ShowWindow(validateResult);
        //         return;
        //     }
        //
        //     // Viven Object를 빌드합니다.
        //     // var result = VivenBuildManager.TryBuildVObject(cttId, cttBinVal, selected);
        //     var buildData = ScriptableObject.CreateInstance<VivenObjectBuildData>();
        //
        //     buildData.gameObject = selected;
        //     buildData.WIN.enabled = true;
        //     buildData.WIN.targetPath = AssetDatabase.GetAssetPath(selected);
        //
        //     var result = VivenBuildManager.TryBuildVObject(buildData, VivenPlatform.WIN, "");
        //     // 빌드 결과를 표시합니다.
        //     BuildResultWindow.ShowWindow(result);
        // }

        // /// <summary>
        // /// Prefab을 선택했을 때만 옵션이 활성화되도록 합니다.
        // /// </summary>
        // /// <returns>옵션이 선택 가능한지 여부</returns>
        // [MenuItem("Assets/Viven/Quick Build Viven Object", true)]
        // private static bool QuickBuildVivenObjectValidation()
        // {
        //     Object selected = Selection.activeObject;
        //     return selected != null && PrefabUtility.IsPartOfPrefabAsset(selected);
        // }
    }
}