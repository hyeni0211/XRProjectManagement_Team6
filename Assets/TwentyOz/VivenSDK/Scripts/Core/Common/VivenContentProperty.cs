using System;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Common
{
    /// <summary>
    /// Map Property의 Key, Value를 저장하는 구조체
    /// </summary>
    [Serializable]
    public struct VivenContentProperty
    {
        [SerializeField] public string propertyName;
        [SerializeField] public string propertyValue;
    }
}