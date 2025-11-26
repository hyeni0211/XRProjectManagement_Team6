using System;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VMap;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// VivenSDK의 빌드 설정을 관리하는 ScriptableObject 클래스입니다.
    /// 전역 빌드 설정과 VMap 빌드 설정을 제공합니다.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "VivenSDK/BuildSettings")]
    [Serializable]
    public class VivenBuildSetting : ScriptableObject
    {
        /// <summary>
        /// 플랫폼별 빌드 프로파일 정보를 담고 있는 설정
        /// </summary>
        [SerializeField] public VivenContentBuildProfiles contentBuildProfiles;
        
        /// <summary>
        /// VMap 빌드 관련 설정
        /// </summary>
        [SerializeField] public VMapBuildSetting vMapBuildSetting;

        /// <summary>
        /// VivenBuildSetting의 싱글톤 인스턴스
        /// </summary>
        private static VivenBuildSetting _instance;

        /// <summary>
        /// 전역 빌드 설정 인스턴스를 가져옵니다.
        /// 인스턴스가 없으면 자동으로 생성됩니다.
        /// </summary>
        public static VivenBuildSetting Global
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<VivenBuildSetting>("Assets/TwentyOz/VivenSDK/Scripts/Editor/Build/Datas/BuildSettings.asset");
                }

                return _instance;
            }
        }
        
        /// <summary>
        /// VMap 빌드 설정을 가져옵니다.
        /// </summary>
        public static VMapBuildSetting VMapBuildSetting => Global.vMapBuildSetting;
    }
}