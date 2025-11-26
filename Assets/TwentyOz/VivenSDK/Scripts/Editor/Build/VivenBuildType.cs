// ReSharper disable StringLiteralTypo
namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    /// <summary>
    /// VivenSDK에서 지원하는 빌드 타입을 정의하는 열거형입니다.
    /// </summary>
    public enum VivenBuildType
    {
        /// <summary>
        /// Viven 오브젝트 빌드 타입
        /// </summary>
        vobject,
        
        /// <summary>
        /// Viven 맵 빌드 타입
        /// </summary>
        vmap,
        
        /// <summary>
        /// Viven 아바타 빌드 타입
        /// </summary>
        vavatar,
    }

    /// <summary>
    /// VivenBuildType 열거형에 대한 확장 메서드를 제공하는 정적 클래스입니다.
    /// </summary>
    public static class VivenBuildTypeExtension
    {
        /// <summary>
        /// 빌드 타입에 해당하는 파일 확장자를 반환합니다.
        /// </summary>
        /// <param name="vivenBuildType">빌드 타입 열거형 값</param>
        /// <returns>파일 확장자 (예: "vobj", "vmap", "vavt")</returns>
        public static string GetExtension(this VivenBuildType vivenBuildType)
        {
            return vivenBuildType switch
            {
                VivenBuildType.vobject => "vobj",
                VivenBuildType.vmap => "vmap",
                VivenBuildType.vavatar => "vavt",
                _ => throw new System.Exception("Invalid Build Type")
            };
        }

        /// <summary>
        /// 빌드 타입의 표시 이름을 반환합니다.
        /// </summary>
        /// <param name="vivenBuildType">빌드 타입 열거형 값</param>
        /// <returns>표시 이름 (예: "VivenObject", "VivenMap", "VivenAvatar")</returns>
        public static string GetName(this VivenBuildType vivenBuildType)
        {
            return vivenBuildType switch
            {
                VivenBuildType.vobject => "VivenObject",
                VivenBuildType.vmap => "VivenMap",
                VivenBuildType.vavatar => "VivenAvatar",
                _ => throw new System.Exception("Invalid Build Type")
            };
        }
        
        /// <summary>
        /// 빌드 타입에 해당하는 Addressable 그룹 이름을 반환합니다.
        /// </summary>
        /// <param name="vivenBuildType">빌드 타입 열거형 값</param>
        /// <returns>Addressable 그룹 이름</returns>
        public static string GetGroupName(this VivenBuildType vivenBuildType)
        {
            return vivenBuildType switch
            {
                VivenBuildType.vobject => "VivenObject",
                VivenBuildType.vmap => "VivenMap",
                VivenBuildType.vavatar => "VivenAvatar",
                _ => throw new System.Exception("Invalid Build Type")
            };
        }
        
        /// <summary>
        /// 빌드 타입에 해당하는 Addressable 디렉토리 이름을 반환합니다.
        /// </summary>
        /// <param name="vivenBuildType">빌드 타입 열거형 값</param>
        /// <returns>Addressable 디렉토리 이름 (예: "obj", "map", "avatar")</returns>
        public static string GetAddressableDirectory(this VivenBuildType vivenBuildType)
        {
            return vivenBuildType switch
            {
                VivenBuildType.vobject => "obj",
                VivenBuildType.vmap => "map",
                VivenBuildType.vavatar => "avatar",
                _ => throw new System.Exception("Invalid Build Type")
            };
        }
    }
}