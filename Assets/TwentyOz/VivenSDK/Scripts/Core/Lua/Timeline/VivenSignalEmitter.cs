using UnityEngine;
using UnityEngine.Timeline;

namespace TwentyOz.VivenSDK.Scripts.Core.Lua.Timeline
{
    /// <summary>
    /// VivenSDK 용 SignalEmitter
    /// </summary>
    /// <remarks>
    /// Timeline에서 SignalEmitter로 VivenBehaviour에 이벤트를 전달할 경우 VivenSignalEmitter를 사용해야 합니다.
    /// Signal을 수신할 VivenBehaviour는 동일한 GameObject에 <see cref="VivenSignalReceiver"/> 컴포넌트가 있어야 합니다.
    /// VivenBehaviour에 <c>methodName</c>와 동일한 이름의 함수가 존재한다면 해당 함수가 호출됩니다.
    /// </remarks>
    public class VivenSignalEmitter : SignalEmitter
    {
        [SerializeField] public string methodName;
    }
}