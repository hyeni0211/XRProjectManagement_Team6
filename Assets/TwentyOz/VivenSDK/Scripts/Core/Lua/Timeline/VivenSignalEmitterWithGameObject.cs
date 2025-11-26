using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Lua.Timeline
{
    /// <summary>
    /// Signal Emitter에 GameObject를 전달합니다.
    /// </summary>
    /// <inheritdoc/>
    public class VivenSignalEmitterWithGameObject : VivenSignalEmitter
    {
        [SerializeField] public GameObject value;
    }
}