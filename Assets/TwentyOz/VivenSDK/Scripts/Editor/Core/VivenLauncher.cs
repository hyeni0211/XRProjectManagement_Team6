using System;
using System.Diagnostics;
using Microsoft.Win32;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using TwentyOz.VivenSDK.Scripts.Editor.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Application = UnityEngine.Device.Application;
using Debug = UnityEngine.Debug;

namespace TwentyOz.VivenSDK.Scripts.Editor.Core
{
    /// <summary>
    /// Viven 사이트와 통신해 사용자 정보를 가져오거나, Viven 실행파일을 실행하는 클래스
    /// </summary>
    public static class VivenLauncher
    {
        /// <summary>
        /// Viven 실행파일 경로
        /// </summary>
        private static string _vivenPath;
        
        /// <summary>
        /// 로그인 여부
        /// </summary> 
        public static bool IsLogin()
        {
            // user-domain이 기존에 설정되었다면 CurrentDomain 불러오기
            if (EditorPrefs.HasKey("user-domain"))
            {
                // user-domain 이 LoginDomain에 정의된 값인 지 확인
                if (Enum.IsDefined(typeof(LoginDomain), EditorPrefs.GetInt("user-domain")))
                {
                    VivenDomain.SetDomain((LoginDomain)EditorPrefs.GetInt("user-domain"));
                }
            }
            return EditorPrefs.HasKey("user-token") && VivenDomain.CurrentDomain != LoginDomain.None;
        }

        /// <summary>
        /// 로그인된 사용자 정보
        /// </summary>
        private static UserInfo _userInfo;

        private static UnityWebRequest _request;
        public static UserInfo GetUserInfo() => _userInfo;

        
        /// <summary>
        /// 사용자 로그인 시키기
        /// </summary>
        public static void Login()
        {
            // 새로운 윈도우를 열어서 로그인을 하도록 사용자 유도를 함.
            VivenLoginWindow.ShowWindow();
        }

        /// <summary>
        /// 사용자 로그아웃 시키기
        /// </summary>
        public static void Logout()
        {
            var request = VivenAPI.Logout();
            request.SetRequestHeader("Authorization", "Bearer " + EditorPrefs.GetString("user-token"));
            request.SendWebRequest().completed += operation =>
            {
                EditorPrefs.DeleteKey("user-token");
                EditorPrefs.DeleteKey("user-domain");
                _userInfo = default;
            };
        }
        
        /// <summary>
        /// Viven 웹과 연결할 수 있는 지 확인합니다.
        /// </summary>
        /// <returns>로그인 여부</returns>
        public static bool ValidateVivenConnection()
        {
            // Check internet connection
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("인터넷 연결이 필요합니다.");
                EditorUtility.DisplayDialog("Error", "인터넷 연결이 필요합니다.", "OK");
                return false;
            }

            if (!IsLogin())
            {
                Debug.LogError("로그인이 필요합니다.");
                EditorUtility.DisplayDialog("Error", "로그인이 필요합니다.", "OK");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Viven Launcher를 통해 Viven 실행
        /// </summary>
        public static void PlayVivenLocal()
        {
            const string registryPath = @"HKEY_CLASSES_ROOT\viven\shell\open\command";
            var          val          = Registry.GetValue(registryPath, "", "").ToString();
            var vivenPath = val.Remove(val.Length - 5, 5).Replace("\"", "");
            var processInfo = new ProcessStartInfo
            {
                Arguments       = $"viven://{VivenDomain.WebURL.GetDomainWebURL()}?d={VivenDomain.DTS.GetDomainDTS()}&t={EditorPrefs.GetString("user-token")}&s=true",
                UseShellExecute = true,
                FileName        = vivenPath,
            };
            Debug.Log(vivenPath);
            Process.Start(processInfo);
        }

        /// <summary>
        /// 로그인 되어있다면 웹에서 User Profile을 가져와 User Info를 업데이트한다.
        /// </summary>
        public static void UpdateUserProfile()
        {
            // 로그인되어있고 인터넷 연결이 되어있다면
            if (IsLogin() && Application.internetReachability != NetworkReachability.NotReachable)
            {
                // 사용자 정보가 없다면 가져오기
                if (!Equals(_userInfo, default(UserInfo)))
                    return;
                
                if(_request != null && _request.isDone)
                    return;
                // 설정된 User Profile을 가져온다.
                _request = VivenAPI.GetUserProfile(EditorPrefs.GetString("user-token"));

                // 웹에서 User Profile을 가져온다.
                _request.SendWebRequest().completed += _ => GetUserInfo(_request);
            }
        }

        private static void GetUserInfo(UnityWebRequest request)
        {
            if (string.IsNullOrEmpty(request.downloadHandler.text))
            {
                Debug.LogError("User Profile을 가져오는 데 실패했습니다. 로그아웃합니다.");
                EditorPrefs.DeleteKey("user-token");
                VivenEditorUtil.RepaintToolbar();
                return;
            }
            _userInfo = JsonUtility.FromJson<UserInfo>(request.downloadHandler.text);
            VivenEditorUtil.RepaintToolbar();
        }
    }
}