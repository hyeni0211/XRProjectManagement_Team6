using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    [CreateAssetMenu(menuName = "VivenSDK/Build/VivenObjectBuildData")]
    public class VivenObjectBuildData : VivenBuildData
    {
        public override VivenBuildType BuildType => VivenBuildType.vobject;

        /// <summary>
        /// 빌드할 게임 오브젝트
        /// </summary>
        [SerializeField] public GameObject gameObject;
        
        public override string GetBuildName()
        {
            return gameObject.name;
        }
        
        public static VivenObjectBuildData Get(GameObject gameObject)
        {
            var buildData = CreateInstance<VivenObjectBuildData>();
            buildData.gameObject = gameObject;
            buildData.WIN = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(gameObject)
            };
            buildData.MAC = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(gameObject)
            };
            buildData.AOS = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(gameObject)
            };
            buildData.IOS = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(gameObject)
            };
            buildData.WEB = new PlatformWrapper
            {
                enabled = true,
                targetPath = AssetDatabase.GetAssetPath(gameObject)
            };
            return buildData;
        }
    }
}