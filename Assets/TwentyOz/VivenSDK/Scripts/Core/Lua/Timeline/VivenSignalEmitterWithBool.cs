using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Lua.Timeline
{
    /// <summary>
    /// VivenSignalEmitter에 bool 값을 추가한 클래스입니다.
    /// </summary>
    /// <inheritdoc/>
    public class VivenSignalEmitterWithBool : VivenSignalEmitter
    {
        [SerializeField] public bool value;
    }
}