using TwentyOz.VivenSDK.Scripts.Editor.Build;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VAvatar;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.UI.Build.Avatar
{
    /// <summary>
    /// Viven Avatar 빌드창입니다
    /// </summary>
    public class VivenAvatarBuildWindow : EditorWindow
    {
        private static VivenBuildSetting BuildSetting => VivenBuildSetting.Global;
        
        /// <summary>
        /// 선택된 Prefab
        /// </summary>
        private static GameObject _prefab;

        /// <summary>
        /// Viven Avatar 빌드창을 엽니다.
        /// </summary>
        /// <param name="prefab"></param>
        public static void ShowWindow(GameObject prefab)
        {
            _prefab = prefab;
            var window = GetWindow<VivenAvatarBuildWindow>();
            window.titleContent = new GUIContent("Viven Avatar Build Setting");
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
                // Viven Avatar를 빌드합니다.
                var buildData = VivenAvatarBuildData.Get(_prefab);
                
                // VObject 빌드
                var result = VivenBuildManager.TryBuildBundle(VivenBuildType.vavatar, buildData, BuildSetting.contentBuildProfiles);

                
                // 빌드 결과를 표시합니다.
                BuildResultWindow.ShowWindow(result);

                // if Build is successful, close the window
                Close();
            }
        }
    }
}