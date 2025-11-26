using TwentyOz.VivenSDK.Scripts.Core.Lua;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Lua
{
    /// <summary>
    /// GameObject 생성 메뉴에서 VivenLuaBehaviour 컴포넌트를 가진 GameObject를 생성하는 메뉴
    /// </summary>
    public static class VivenLuaBehaviourCreator
    {
        [MenuItem("GameObject/Create Other/VivenLuaBehaviour")]
        public static void MakeLuaBehaviour()
        {
            var go = new GameObject("VivenLuaBehaviour");
            go.AddComponent<VivenLuaBehaviour>();
            Selection.activeGameObject = go;
            
        } 
    }
}
