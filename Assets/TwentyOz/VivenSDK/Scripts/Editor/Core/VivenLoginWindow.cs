using System;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using TwentyOz.VivenSDK.Scripts.Editor.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace TwentyOz.VivenSDK.Scripts.Editor.Core
{
    /// <summary>
    /// Viven 로그인 창입니다.
    /// </summary>
    public class VivenLoginWindow : EditorWindow
    {
        private string      _id       = "";
        private string      _password = "";
        private LoginDomain _domain;
        
        /// <summary>
        /// VivenLoginWindow를 엽니다.
        /// </summary>
        public static void ShowWindow()
        {
            var window = GetWindow<VivenLoginWindow>();
            // window size height
            window.minSize      = new Vector2(300, 50);
            window.maxSize      = new Vector2(300, 100);
            window.titleContent = new GUIContent("Login");
            window.Show();
        }

        /// <summary>
        /// GUI 이벤트 시 호출
        /// </summary>
        private void OnGUI()
        {
            CreateGUI();
        }

        /// <summary>
        /// 로그인 창의 GUI를 생성합니다.
        /// </summary>
        private void CreateGUI()
        {
            // Login UI를 구현
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Viven Login");
            
            _id       = EditorGUILayout.TextField("ID", _id);
            _password = EditorGUILayout.PasswordField("Password", _password);
            var options = Enum.GetNames(typeof(LoginDomain));
            _domain = (LoginDomain)EditorGUILayout.Popup("Domain", (int)_domain, options);
            
            if (_domain != VivenDomain.CurrentDomain)
            {
                VivenDomain.SetDomain(_domain);
                EditorPrefs.SetInt("user-domain", (int)VivenDomain.CurrentDomain);
                
                var token = EditorPrefs.GetString("user-token");
                if (!string.IsNullOrEmpty(token))
                {
                    EditorPrefs.DeleteKey("user-token");
                    EditorPrefs.DeleteKey("user-domain");
                }   
            }

            if (GUILayout.Button("Login"))
            {
                if (_domain == LoginDomain.None)
                {
                    EditorUtility.DisplayDialog("도메인 오류", "도메인 설정을 해주세요.", "OK");
                    EditorGUILayout.EndVertical();
                    // refresh gui
                    Repaint();
                    return; // 밑에 실행 안되게 하기
                }
                
                var formData = new WWWForm();
                formData.AddField("loginId", _id);
                formData.AddField("pw", _password);
                var request = VivenAPI.GetLoginToken(formData);
                request.redirectLimit = 0;
                request.SendWebRequest().completed += operation =>
                {
                    // Redirection 처리
                    if (request.responseCode is 301 or 302 or 307 or 308)
                    {
                        var test = request.GetRequestHeader("Location");
                        var redirectPath = request.GetResponseHeader("Location");
                        var redirectRequest = UnityWebRequest.Post(redirectPath, formData);
                        redirectRequest.SendWebRequest().completed += _ =>
                        {
                            if (redirectRequest.result != UnityWebRequest.Result.Success)
                            {
                                EditorUtility.DisplayDialog("로그인에 실패하였습니다.", redirectRequest.downloadHandler.text, "OK");
                                // IsLogin = false;
                                EditorPrefs.DeleteKey("user-token");
                                // refresh gui
                                Repaint();
                                return; // 밑에 실행 안되게 하기
                            }
                            
                            var tokenData = JsonUtility.FromJson<LoginToken>(redirectRequest.downloadHandler.text);
                            // IsLogin = true;
                            EditorPrefs.SetString("user-token", tokenData.token);
                            EditorPrefs.SetInt("user-domain", (int)VivenDomain.CurrentDomain);
                            VivenEditorUtil.RepaintToolbar();
                            
                        };
                        return;
                    }
                    
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        EditorUtility.DisplayDialog("로그인에 실패하였습니다.", request.downloadHandler.text, "OK");
                        // IsLogin = false;
                        EditorPrefs.DeleteKey("user-token");
                        // refresh gui
                        Repaint();
                        return; // 밑에 실행 안되게 하기
                    }

                    var tokenData = JsonUtility.FromJson<LoginToken>(request.downloadHandler.text);
                    // IsLogin = true;
                    EditorPrefs.SetString("user-token", tokenData.token);
                    EditorPrefs.SetInt("user-domain", (int)VivenDomain.CurrentDomain);
                    VivenEditorUtil.RepaintToolbar();
                };

                Close();
            }

            EditorGUILayout.EndVertical();

            // refresh gui
            Repaint();
        }
    }
}