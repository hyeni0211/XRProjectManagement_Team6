#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Lua
{
    /// <summary>
    /// Viven Lua Scriptable Object
    /// </summary>
    public class VivenScript : ScriptableObject
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VivenScriptable")]
        public static void CreateAsset()
        {
            // create .lua file in Current Active Folder
            ProjectWindowUtil.CreateAssetWithContent("NewLuaFile.lua", "");
        }
#endif

        /// <summary>
        /// Lua Script String
        /// </summary>
        public string scriptString;
    }
}