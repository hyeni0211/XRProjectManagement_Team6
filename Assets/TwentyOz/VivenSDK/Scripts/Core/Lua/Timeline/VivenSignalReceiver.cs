using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TwentyOz.VivenSDK.Scripts.Core.Lua.Timeline
{
    /// <summary>
    /// VivenSDK에서 사용하는 SignalReceiver
    /// </summary>
    /// <remarks>
    /// VivenBehaviour 에서 Signal을 수신하려면 VivenSignalReceiver 컴포넌트가 같은 GameObject에 있어야 합니다.
    /// VivenSignalReceiver 컴포넌트는 Signal을 수신해 LuaBehaviour에 전달합니다.
    /// </remarks>
    public class VivenSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField] public VivenLuaBehaviour vivenLuaBehaviour;
        [SerializeField] public SignalAsset signalAsset;

        public void OnNotify(Playable origin, INotification notification, object context)
        {
        }
    }
}