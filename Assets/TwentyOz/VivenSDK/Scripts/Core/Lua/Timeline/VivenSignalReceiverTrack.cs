using UnityEngine.Timeline;

namespace TwentyOz.VivenSDK.Scripts.Core.Lua.Timeline
{
    /// <summary>
    /// Timeline에서 SignalReceiver를 사용하기 위한 Track
    /// </summary>
    [TrackClipType(typeof(VivenSignalReceiver))]
    [TrackBindingType(typeof(VivenSignalReceiver))]
    public class VivenSignalReceiverTrack : TrackAsset
    {
    }
}