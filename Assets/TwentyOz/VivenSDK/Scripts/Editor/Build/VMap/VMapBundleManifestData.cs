using System.Collections.Generic;
using System.Linq;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VMap
{
    public struct VMapBundleManifestData
    {
        public List<string> AvailablePlatforms;
        
        public string ToJson()
        {
            var json = "{";
            json += $"\"availablePlatforms\": [{string.Join(",", AvailablePlatforms.Select(platform => $"\"{platform}\""))}]";
            json += "}";
            return json;
        }
    }
}