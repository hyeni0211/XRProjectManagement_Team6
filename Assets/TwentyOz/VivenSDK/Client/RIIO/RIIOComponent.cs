using System;
using UnityEngine;

namespace Twoz.Viven.RuntimeInteractionObject.Core
{
    /// <summary>
    /// {RIIO 에 대한 설명}
    /// </summary>
    /// <remarks>
    /// <see cref="RIIOSender"/>와 <see cref="RIIOReceiver"/>를 관리하는 컴포넌트입니다.
    /// <para>
    /// RIIOComponent를 다른 RIIOComponent와 연결할 경우 Key를 사용해 Sender와 Receiver를 탐색합니다.
    /// 따라서 RIIOComponent에서 관리하는 Sender와 Receiver는 고유한 Key 값을 가져야 합니다.
    /// RIIOComponent에 추가하지 않은 Sender와 Receiver는 VivenBehaviour에서 사용할 수 없습니다.
    /// </para>
    /// <para>
    /// RIIOComponent는 VivenBehaviour에서 사용됩니다.
    /// 다른 컴포넌트와 연결해 데이터를 송수신, 처리할 수 있습니다.
    /// RIIOComponent는 다음과 같은 방법으로 사용할 수 있습니다.
    /// 
    /// <code language="lua">
    ///     function start()
    ///        ---@type RIIOComponent
    ///        riioComponent = self.gameObject:GetComponent("RIIOComponent")
    /// 
    ///        ---@type RIIOSender
    ///        outSig = riioComponent:GetSender("Output")
    /// 
    ///        ---@type RIIOReceiver
    ///        inSig1 = riioComponent:GetReceiver("Input1")
    ///        inSig1:NewInput('+', OnTriggered_In1)
    /// 
    ///        ---@type RIIOReceiver
    ///        inSig2 = riioComponent:GetReceiver("Input2")
    ///        inSig2:NewInput('+', OnTriggered_In2)
    ///     end
    /// </code>
    /// 
    /// 각 Sender와 Receiver 객체에 Key 값을 통해 접근하고 InputHandler를 등록해 값을 가져옵니다.
    /// InputHandler는 데이터에 변경이 생겼을 경우에만 호출됩니다. 동일한 값이 여러 번 들어오는 경우 최적화를 위해 호출되지 않습니다.
    /// 지속적으로 이벤트를 받기 위해서는 RIIODataType을 Void로 설정해야 합니다.
    /// </para>
    /// <para>
    /// RIIOComponent들의 연결 상태는 네트워크를 통해 동기화됩니다. 다른 사용자가 연결을 해제하거나 새로운 연결을 추가하면 모든 사용자에게 동기화됩니다.
    /// RIIOComponent에서 송수신하는 데이터는 동기화되지만, VivenBehaviour의 상태는 동기화되지 않습니다. RPC, SyncData를 사용하십시오.
    /// </para>
    /// </remarks>
    [AddComponentMenu("VivenSDK/RIIO/RIIO Component")]
    public class RIIOComponent : MonoBehaviour
    {
        public ReceiverData[] receiverList;
        public SenderData[] senderList;
    }

    /// <summary>
    /// RIIOComponent에 등록된 Receiver 정보입니다.
    /// <br/>Key 값으로 Receiver를 찾습니다.
    /// </summary>
    [Serializable]
    public struct ReceiverData
    {
        /// <summary> RIIOComponent에서 Receiver를 찾기 위한 Key 값 </summary>
        public string key;
        /// <summary> Receiver </summary>
        public RIIOReceiver receiver;
    }
    
    /// <summary>
    /// RIIOComponent에 등록된 Sender 정보입니다.
    /// <br/>Key 값으로 Receiver를 찾습니다.
    /// </summary>
    [Serializable]
    public struct SenderData
    {
        /// <summary> RIIOComponent에서 Sender를 찾기 위한 Key 값 </summary>
        public string key;
        /// <summary> Sender </summary>
        public RIIOSender sender;
    }
}