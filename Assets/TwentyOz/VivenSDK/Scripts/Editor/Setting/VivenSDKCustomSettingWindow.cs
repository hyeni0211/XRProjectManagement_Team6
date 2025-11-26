using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Setting
{
    public class VivenSDKCustomSettingsWindow : EditorWindow
    {
        private bool _isProjectSettingsApplied;
        private bool _isAddressableSettingsApplied;
        
        //TODO : git 으로 변경하면 경로 바꿔줘야함
        private const string SettingsSourcePath = "Assets/TwentyOz/Settings/ProjectSettings/";
        private const string SettingsTargetPath = "ProjectSettings/";
        
        public Texture2D logo;
        
        private GUIStyle _buttonGUIStyle;
        private readonly GUILayoutOption[] _buttonStyle = { GUILayout.Width(350), GUILayout.Height(50) };
        
        [MenuItem("VIVEN SDK/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<VivenSDKCustomSettingsWindow>("VIVEN SDK Settings");
            window.minSize = new Vector2(400, 400);
            window.maxSize = new Vector2(400, 401);
            window.Show();
        }
        
        private void OnEnable()
        {
            //TODO : : git 으로 변경하면 경로 바꿔줘야함
            logo = (Texture2D)EditorGUIUtility.Load("Assets/TwentyOz/VivenSDK/Logo/Logo_horizontal.png");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(50);
            
            if (logo != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(logo, GUILayout.Width(300), GUILayout.Height(100));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(50);
            
            DrawButton("Project Settings 적용", ApplyProjectSettings, _isProjectSettingsApplied);
            
            EditorGUILayout.Space();
            
            DrawButton("Addressable Settings 적용", ApplyAddressableSettings, _isAddressableSettingsApplied);
            
            EditorGUILayout.Space();
            
            DrawButton("Close", Close);
        }
        
        private void DrawButton(string label, Action action, bool? isApplied = null)
        {
            if (isApplied.HasValue)
            {
                if (isApplied.Value)
                {
                    _buttonGUIStyle = new GUIStyle(GUI.skin.button);
                    _buttonGUIStyle.normal.background = MakeTexture(1, 1, Color.green);
                    _buttonGUIStyle.normal.textColor = Color.black;
                    _buttonGUIStyle.fontStyle = FontStyle.Bold;
                    _buttonGUIStyle.fontSize = 14;
                }
                else
                {
                    _buttonGUIStyle = new GUIStyle(GUI.skin.button);
                    _buttonGUIStyle.fontStyle = FontStyle.Bold;
                    _buttonGUIStyle.fontSize = 14;
                }
            }
            else
            {
                _buttonGUIStyle = new GUIStyle(GUI.skin.button);
                _buttonGUIStyle.fontStyle = FontStyle.Bold;
                _buttonGUIStyle.fontSize = 14;
            }
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(label, _buttonGUIStyle, _buttonStyle))
            {
                action();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private void ApplyProjectSettings()
        {
            // 설정을 적용할 경로 확인
            if (!Directory.Exists(SettingsSourcePath))
            {
                Debug.LogError("Recommended settings not found at " + SettingsSourcePath);
                return;
            }

            // 파일을 ProjectSettings 폴더로 복사
            CopySettingsFile("Player.asset");
            CopySettingsFile("GraphicsSettings.asset");
            CopySettingsFile("QualitySettings.asset");
            CopySettingsFile("TagManager.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            _isProjectSettingsApplied = true;
            
            Debug.Log("Recommended settings applied.");
        }
        
        private void CopySettingsFile(string fileName)
        {
            var sourceFile = Path.Combine(SettingsSourcePath, fileName);
            var targetFile = Path.Combine(SettingsTargetPath, fileName);

            // 파일이 존재하면 복사
            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, targetFile, true);
                Debug.Log($"Copied {fileName} to {SettingsTargetPath}");
            }
            else
            {
                Debug.LogWarning($"{fileName} not found in {SettingsSourcePath}");
            }
        }
        
        private void ApplyAddressableSettings()
        {
            // Get the current Addressables settings (or create a new one if it doesn't exist)
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            if (settings == null)
            {
                Debug.LogError("Failed to create or find Addressables settings.");
                return;
            }
            
            settings.EnableJsonCatalog = true;
            Debug.Log("EnableJsonCatalog set to true.");
            
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            
            _isAddressableSettingsApplied = true;

            Debug.Log("Addressables settings created successfully."); ;
        }
        
    }
}