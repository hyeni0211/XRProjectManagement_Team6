using System.Reflection;

namespace TwentyOz.VivenSDK.Scripts.Editor.Util
{
    public static class VivenEditorUtil
    {
        public static void RepaintToolbar()
        {
            var toolbarType = typeof(UnityEditor.
                Editor).Assembly.GetType("UnityEditor.Toolbar");
            if (toolbarType == null) return;

            var miRepaintToolBar = toolbarType.GetMethod("RepaintToolbar", BindingFlags.NonPublic | BindingFlags.Static);
            if (miRepaintToolBar == null) return;
            
            miRepaintToolBar.Invoke(null, null);
        }
    }
}