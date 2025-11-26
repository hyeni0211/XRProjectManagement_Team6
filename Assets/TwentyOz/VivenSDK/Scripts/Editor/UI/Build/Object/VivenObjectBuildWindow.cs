using TwentyOz.VivenSDK.Scripts.Editor.UI;
using TwentyOz.VivenSDK.Scripts.Editor.UI.Build;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// Viven Object 빌드창입니다
    /// </summary>
    public class VivenObjectBuildWindow : EditorWindow
    {
        /// <summary>
        /// 선택된 Prefab
        /// </summary>
        private static GameObject _prefab;
        
        /// <summary>
        /// SDK 컨텐츠 빌드 세팅
        /// </summary>
        private static VivenBuildSetting BuildSetting => VivenBuildSetting.Global;

        /// <summary>
        /// Viven Object 빌드창을 엽니다.
        /// </summary>
        /// <param name="prefab"></param>
        public static void ShowWindow(GameObject prefab)
        {
            _prefab = prefab;
            var window = GetWindow<VivenObjectBuildWindow>();
            window.titleContent = new GUIContent("Viven Object Build Setting");
            window.Show();
        }

        /// <summary>
        /// GUI 이벤트 시 호출
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("빌드 설정", EditorStyles.boldLabel);
            if (GUILayout.Button("빌드하기"))
            {
                var buildData = VivenObjectBuildData.Get(_prefab);
                
                // VObject 빌드
                var result = VivenBuildManager.TryBuildBundle(VivenBuildType.vobject, buildData, BuildSetting.contentBuildProfiles);
                
                // 빌드 결과를 표시합니다.
                BuildResultWindow.ShowWindow(result);

                // if Build is successful, close the window
                Close();
            }
        }
    }
}