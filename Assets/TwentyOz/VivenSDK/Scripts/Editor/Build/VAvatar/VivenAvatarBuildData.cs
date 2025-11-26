using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VAvatar
{
    [CreateAssetMenu(menuName = "VivenSDK/Build/VivenAvatarBuildData")]
    public class VivenAvatarBuildData : VivenBuildData
    {
        public override VivenBuildType BuildType => VivenBuildType.vavatar;
        
        /// <summary>
        /// 빌드할 아바타
        /// </summary>
        [SerializeField] public GameObject targetAvatar;

        public override string GetBuildName()
        {
            return targetAvatar.name;    
        }

        public static VivenAvatarBuildData Get(GameObject avatar)
        {
            var buildData = CreateInstance<VivenAvatarBuildData>();
            buildData.targetAvatar = avatar;
            buildData.WIN = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(avatar)
            };
            buildData.MAC = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(avatar)
            };
            buildData.AOS = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(avatar)
            };
            buildData.IOS = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(avatar)
            };
            buildData.WEB = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(avatar)
            };
            return buildData;
        }
    }
}