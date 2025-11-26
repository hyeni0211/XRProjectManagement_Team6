using System;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// VivenSDK에서 지원하는 플랫폼을 정의하는 열거형입니다.
    /// </summary>
    public enum VivenPlatform
    {
        /// <summary>
        /// Windows 플랫폼
        /// </summary>
        WIN,
        
        /// <summary>
        /// macOS 플랫폼
        /// </summary>
        MAC,
        
        /// <summary>
        /// Android 플랫폼
        /// </summary>
        AOS,
        
        /// <summary>
        /// iOS 플랫폼
        /// </summary>
        IOS,
        
        /// <summary>
        /// Web VRM 뷰어 플랫폼
        /// </summary>
        WEB,
    }

    /// <summary>
    /// VivenPlatform 열거형에 대한 확장 메서드를 제공하는 정적 클래스입니다.
    /// </summary>
    public static class VivenPlatformExtension
    {
        /// <summary>
        /// 플랫폼의 대문자 이름을 반환합니다.
        /// </summary>
        /// <param name="vivenPlatform">플랫폼 열거형 값</param>
        /// <returns>플랫폼의 대문자 이름 (예: "WIN", "MAC" 등)</returns>
        public static string GetPlatformName(this VivenPlatform vivenPlatform)
        {
            return vivenPlatform switch
            {
                VivenPlatform.WIN => "WIN",
                VivenPlatform.MAC => "MAC",
                VivenPlatform.AOS => "AOS",
                VivenPlatform.IOS => "IOS",
                VivenPlatform.WEB => "WEB",
                _ => throw new Exception("Invalid Platform")
            };
        }

        /// <summary>
        /// 플랫폼의 소문자 디렉토리 이름을 반환합니다.
        /// </summary>
        /// <param name="vivenPlatform">플랫폼 열거형 값</param>
        /// <returns>플랫폼의 소문자 디렉토리 이름 (예: "win", "mac" 등)</returns>
        public static string GetPlatformDirectory(this VivenPlatform vivenPlatform)
        {
            return vivenPlatform switch
            {
                VivenPlatform.WIN => "win",
                VivenPlatform.MAC => "mac",
                VivenPlatform.AOS => "aos",
                VivenPlatform.IOS => "ios",
                VivenPlatform.WEB => "web",
                _ => throw new Exception("Invalid Platform")
            };
        }

        /// <summary>
        /// 지원되는 모든 플랫폼 목록
        /// </summary>
        public static readonly VivenPlatform[] Platforms =
            new[]
            {
                VivenPlatform.WIN,
                VivenPlatform.MAC,
                VivenPlatform.AOS,
                VivenPlatform.IOS,
                VivenPlatform.WEB,
            };
    }
}