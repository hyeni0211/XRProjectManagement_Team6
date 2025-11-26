using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// 플랫폼별 빌드 설정을 래핑하는 클래스입니다.
    /// 각 플랫폼의 빌드 활성화 여부와 대상 경로를 관리합니다.
    /// </summary>
    [Serializable]
    public class PlatformWrapper
    {
        /// <summary>
        /// 해당 플랫폼의 빌드 활성화 여부
        /// </summary>
        [SerializeField] public bool enabled;
        
        /// <summary>
        /// 빌드 대상 경로
        /// </summary>
        [FormerlySerializedAs("scenePath")] [SerializeField] public string targetPath;
    }
}