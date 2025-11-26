using System;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using TwentyOz.VivenSDK.Scripts.Editor.Build.VMap;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// VivenSDK의 빌드 데이터를 관리하는 추상 클래스입니다.
    /// VMap, VObject, VAvatar 등의 빌드 데이터의 기본 클래스로 사용됩니다.
    /// </summary>
    public abstract class VivenBuildData : ScriptableObject
    {
        /// <summary>
        /// 빌드 컨텐츠의 종류를 반환합니다.
        /// </summary>
        public abstract VivenBuildType BuildType { get; }

        /// <summary>
        /// Windows 플랫폼용 빌드 설정 래퍼
        /// </summary>
        [SerializeField] public PlatformWrapper WIN;

        /// <summary>
        /// macOS 플랫폼용 빌드 설정 래퍼
        /// </summary>
        [SerializeField] public PlatformWrapper MAC;

        /// <summary>
        /// Android 플랫폼용 빌드 설정 래퍼
        /// </summary>
        [SerializeField] public PlatformWrapper AOS;

        /// <summary>
        /// iOS 플랫폼용 빌드 설정 래퍼
        /// </summary>
        [SerializeField] public PlatformWrapper IOS;

        /// <summary>
        /// Web 플랫폼용 빌드 설정 래퍼
        /// </summary>
        [SerializeField] public PlatformWrapper WEB;

        /// <summary>
        /// 지정된 플랫폼에 해당하는 빌드 설정 래퍼를 반환합니다.
        /// </summary>
        /// <param name="platform">Viven 플랫폼 열거형 값</param>
        /// <returns>해당 플랫폼의 빌드 설정 래퍼</returns>
        public PlatformWrapper GetPlatformSceneWrapper(VivenPlatform platform)
        {
            switch (platform)
            {
                case VivenPlatform.WIN:
                    return WIN;
                case VivenPlatform.MAC:
                    return MAC;
                case VivenPlatform.AOS:
                    return AOS;
                case VivenPlatform.IOS:
                    return IOS;
                case VivenPlatform.WEB:
                    return WEB;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 빌드 대상의 이름을 반환합니다.
        /// </summary>
        /// <returns>빌드 대상의 이름</returns>
        public abstract string GetBuildName();

        public virtual VivenContentProperty[] GetContentProperties()
        {
            return Array.Empty<VivenContentProperty>();
        }

        /// <summary>
        /// 대상과 함께 빌드할 추가적인 오브젝트를 빌드합니다.
        /// </summary>
        /// <remarks>
        /// 현재 VMap만이 다른 VObject를 함께 빌드할 수 있습니다. <see cref="VivenMapBuildData"/> 참고
        /// </remarks>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual BuildResultData BuildAdditionalObjects(AddressableAssetSettings settings)
        {
            return BuildResultData.Success("nothing", "no additional objects built");
        } 
    }
}