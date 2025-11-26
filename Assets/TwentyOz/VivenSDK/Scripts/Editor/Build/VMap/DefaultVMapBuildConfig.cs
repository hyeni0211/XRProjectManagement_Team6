using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VMap
{
    [CreateAssetMenu(menuName = "VivenSDK/DefaultVMapBuildConfig", fileName = "DefaultVMapBuildConfig", order = 0)]
    public sealed class DefaultVMapBuildConfig : VMapBuildConfig
    {
        public DefaultVMapBuildConfig()
        {
            ConfigName = "Default";
            Condition = () => true;
            Start = () => AdditionalProperties?.Clear();
        }
    }
}