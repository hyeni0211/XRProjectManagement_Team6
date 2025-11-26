using System.Linq;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build
{
    public static class VivenArrayExtension
    {
        public static T FindAnyComponentInChildren<T>(this GameObject[] roots, bool includeInactive = false)
            where T : Component
        {
            return roots
                .Select(root => root.GetComponentInChildren<T>(includeInactive))
                .FirstOrDefault(obj => obj);
        }
    }
}