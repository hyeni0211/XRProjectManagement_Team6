using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VMap
{
    [CreateAssetMenu(menuName = "VivenSDK/Build/Create ViveTestMapBuildData", fileName = "VivenTestMapBuildData",
        order = 0)]
    public class VivenTestMapBuildData : VivenMapBuildData
    {
        private const string AssetPath =
            "Assets/TwentyOz/VivenSDK/Scripts/Editor/Build/VMap/TestOnViven_BuildData.asset";

        // TODO: 추후 플랫폼 정보에 따라 테스트할 수 있도록 수정
        public VivenPlatform CurrentEditorPlatform => VivenPlatform.WIN;

        public void Init()
        {
            WIN.enabled = false;
            WIN.targetPath = string.Empty;
            MAC.enabled = false;
            MAC.targetPath = string.Empty;
            IOS.enabled = false;
            IOS.targetPath = string.Empty;
            AOS.enabled = false;
            AOS.targetPath = string.Empty;
            WEB.enabled = false;
            WEB.targetPath = string.Empty;

            switch (CurrentEditorPlatform)
            {
                case VivenPlatform.WIN:
                    WIN.enabled = true;
                    WIN.targetPath = SceneManager.GetActiveScene().path;
                    break;
                case VivenPlatform.MAC:
                    MAC.enabled = true;
                    MAC.targetPath = SceneManager.GetActiveScene().path;
                    break;
                case VivenPlatform.IOS:
                    IOS.enabled = true;
                    IOS.targetPath = SceneManager.GetActiveScene().path;
                    break;
                case VivenPlatform.AOS:
                    AOS.enabled = true;
                    AOS.targetPath = SceneManager.GetActiveScene().path;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetBuildName()
        {
            return SceneManager.GetActiveScene().name;
        }

        private static VivenTestMapBuildData _instance;

        public static VivenTestMapBuildData Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<VivenTestMapBuildData>(AssetPath);
                    if (!_instance)
                    {
                        _instance = CreateInstance<VivenTestMapBuildData>();
                        AssetDatabase.CreateAsset(_instance, AssetPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }

                return _instance;
            }
        }
    }
}