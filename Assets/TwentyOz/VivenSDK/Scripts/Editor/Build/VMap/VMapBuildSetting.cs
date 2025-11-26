using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VMap
{
    [CreateAssetMenu(fileName = "VMapBuildSettings", menuName = "VivenSDK/VMapBuildSettings")]
    [Serializable]
    public class VMapBuildSetting : ScriptableObject
    {
        /// <summary>
        /// Default 빌드 Config, 반드시 포함되어야 합니다.
        /// </summary>
        [SerializeField] public VMapBuildConfig defaultConfig;
        
        /// <summary>
        /// 빌드 설정 리스트
        /// </summary>
        [FormerlySerializedAs("ConfigList")] [SerializeField] public List<VMapBuildConfig> configList = new();
        
        /// <summary>
        /// 현재 빌드 설정의 인덱스
        /// </summary>
        [HideInInspector] public int currentConfigIndex;
        
        /// <summary>
        /// 플랫폼별 빌드 프로파일 정보
        /// </summary>
        [FormerlySerializedAs("mapBuildProfiles")] [SerializeField] public VivenContentBuildProfiles contentBuildProfiles;
        

        public void OnValidate()
        {
            if(configList.Count == 0 || !configList.Contains(defaultConfig))
                configList.Add(defaultConfig);
        }

        public void AddConfig(VMapBuildConfig config)
        {
            if(configList.Contains(config))
                return;
            configList.Add(config);
        }
    }
}