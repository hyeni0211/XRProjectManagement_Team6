using System;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// VivenSDK의 콘텐츠 빌드 프로파일을 관리하는 ScriptableObject 클래스입니다.
    /// 각 플랫폼별 빌드 프로파일을 저장하고 관리합니다.
    /// </summary>
    [CreateAssetMenu(fileName = "VivenContentBuildProfiles", menuName = "VivenSDK/VivenContentBuildProfiles")]
    [Serializable]
    public class VivenContentBuildProfiles : ScriptableObject
    {
        /// <summary>
        /// Windows 플랫폼용 빌드 프로파일
        /// </summary>
        [SerializeField] public BuildProfile windowBuildProfile;
        
        /// <summary>
        /// macOS 플랫폼용 빌드 프로파일
        /// </summary>
        [SerializeField] public BuildProfile macBuildProfile;
        
        /// <summary>
        /// Android 플랫폼용 빌드 프로파일
        /// </summary>
        [SerializeField] public BuildProfile androidBuildProfile;
        
        /// <summary>
        /// iOS 플랫폼용 빌드 프로파일
        /// </summary>
        [SerializeField] public BuildProfile iosBuildProfile;
        
        /// <summary>
        /// Web 플랫폼용 빌드 프로파일
        /// </summary>
        [SerializeField] public BuildProfile webBuildProfile;

        /// <summary>
        /// VivenContentBuildProfiles의 생성자입니다.
        /// </summary>
        /// <param name="windowBuildProfile">Windows 빌드 프로파일</param>
        /// <param name="macBuildProfile">macOS 빌드 프로파일</param>
        /// <param name="androidBuildProfile">Android 빌드 프로파일</param>
        /// <param name="iosBuildProfile">iOS 빌드 프로파일</param>
        /// <param name="webBuildProfile">Web 빌드 프로파일</param>
        public VivenContentBuildProfiles(BuildProfile windowBuildProfile, BuildProfile macBuildProfile,
            BuildProfile androidBuildProfile, BuildProfile iosBuildProfile, BuildProfile webBuildProfile)
        {
            this.windowBuildProfile = windowBuildProfile;
            this.macBuildProfile = macBuildProfile;
            this.androidBuildProfile = androidBuildProfile;
            this.iosBuildProfile = iosBuildProfile;
            this.webBuildProfile = webBuildProfile;
        }

        /// <summary>
        /// 플랫폼에 해당하는 빌드 프로파일을 가져오거나 설정합니다.
        /// </summary>
        /// <param name="vivenPlatform">Viven 플랫폼 열거형 값</param>
        /// <returns>해당 플랫폼의 빌드 프로파일</returns>
        public BuildProfile this[VivenPlatform vivenPlatform]
        {
            get
            {
                return vivenPlatform switch
                {
                    VivenPlatform.WIN => windowBuildProfile,
                    VivenPlatform.MAC =>
                    #if UNITY_EDITOR_OSX
                        macBuildProfile,
                    #else
                        // Windows 에서 OSX로 빌드 프로파일 변경 시 오류 발생, IOS 프로파일로 대신 빌드
                        iosBuildProfile,
                    #endif
                    VivenPlatform.AOS => androidBuildProfile,
                    VivenPlatform.IOS => iosBuildProfile,
                    VivenPlatform.WEB => webBuildProfile,
                    _ => null
                };
            }
            set
            {
                switch (vivenPlatform)
                {
                    case VivenPlatform.WIN:
                        windowBuildProfile = value;
                        break;
                    case VivenPlatform.MAC:
                    #if UNITY_EDITOR_OSX
                        macBuildProfile = value;
                    #else
                        // Windows 에서 OSX로 빌드 프로파일 변경 시 오류 발생, IOS 프로파일로 대신 빌드
                        iosBuildProfile = value;
                    #endif
                        break;
                    case VivenPlatform.AOS:
                        androidBuildProfile = value;
                        break;
                    case VivenPlatform.IOS:
                        iosBuildProfile = value;
                        break;
                    case VivenPlatform.WEB:
                        webBuildProfile = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(vivenPlatform), vivenPlatform, null);
                }
            }
        }
    }
}