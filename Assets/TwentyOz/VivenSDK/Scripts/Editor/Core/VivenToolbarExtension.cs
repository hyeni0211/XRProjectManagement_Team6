using TwentyOz.VivenSDK.Scripts.Editor.Build;
using TwentyOz.VivenSDK.Scripts.Editor.UI;
using TwentyOz.VivenSDK.Scripts.Editor.UI.Build;
using TwentyOz.VivenSDK.Scripts.Editor.UI.Build.Map;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace TwentyOz.VivenSDK.Scripts.Editor.Core
{
    /// <summary>
    /// Viven SDK의 상단 툴바 클래스
    /// </summary>
    [InitializeOnLoad]
    public class VivenToolbarExtension
    {
        static VivenToolbarExtension()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarRightGUI);
        }

        private static void OnToolbarLeftGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Test on VIVEN", "VIVEN Play")))
            {
                // 일단, 비벤에서 열어서 테스트 하는 기능을 구현할 수가 없음.
                // Local Temp Directory에 V-Map을 만들어서 실행하는 방법을 생각해보자.
                var result =  VivenBuildManager.BuildVMapOnLocalTemp();
                if (result.IsSuccess)
                    VivenLauncher.PlayVivenLocal();
                else
                    BuildResultWindow.ShowWindow(result);
            }
        }
        
        private static void OnToolbarRightGUI()
        {
            if (GUILayout.Button(new GUIContent("Build V-Map", "Build V-Map")))
            {
                VivenMapBuildSettingWindow.ShowWindow();
            }

            GUILayout.FlexibleSpace();
            
            // 로그인되어있다면 웹에서 유저 프로파일을 가져옴
            VivenLauncher.UpdateUserProfile();

            // 로그인되어있다면 유저 닉네임을 표시
            if (VivenLauncher.IsLogin()) 
                GUILayout.Label(VivenLauncher.GetUserInfo().nickname);
            
            // Login 버튼
            if (GUILayout.Button(VivenLauncher.IsLogin() ? "Logout" : "Login"))
            {
                if (VivenLauncher.IsLogin())
                {
                    VivenLauncher.Logout();
                }
                else
                {
                    VivenLauncher.Login();
                }
            }
        }
    }
}