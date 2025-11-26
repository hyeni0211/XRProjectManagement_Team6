using System;
using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields;
using TwentyOz.VivenSDK.Scripts.Core.Lua;
using Twoz.Viven.Interactions;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Network
{
    /// <summary>
    /// VivenSDK에서 사용하는 ObjSync Component
    /// </summary>
    /// <remarks>
    /// VivenBehaviour에서 선언된 변수들은 각 클라이언트에서만 사용됩니다.
    /// 모든 클라이언트에서 상태를 동기화하기 위해서는 <c>VivenCustomSyncView</c>를 사용해 데이터를 동기화해야 합니다.
    /// <br/>
    /// 다음과 같은 동기화를 지원합니다.
    /// 1. 변수 및 프로퍼티 동기화
    /// 2. RPC를 활용한 동기화
    ///
    /// <para>
    /// 변수 및 프로퍼티 동기화는 다음 이벤트들을 기반으로 동작합니다.
    /// <list type="bullet">
    /// <item>
    ///     <term>onSyncViewInitialized</term>
    ///     <description>SyncView 초기화 시에 호출됩니다. 이 시점 이후부터 네트워크와 관련된 동작을 수행 할 수 있습니다.</description>
    /// </item>
    /// <item>
    ///     <term>initializeSync</term>
    ///     <description>서버 혹은 플레이어로부터 데이터를 처음 받아올때 호출됩니다. 해당 구현을 통해 최초 동기화를 위한 작업을 구현해 주세요.</description>
    /// </item>
    /// <item>
    ///     <term>sendSyncUpdate</term>
    ///     <description>데이터를 서버로 전송할때 호출됩니다. 해당 구현을 통해 동기화 할 데이터를 설정해 주세요.</description>
    /// </item>
    /// <item>
    ///     <term>receiveSyncUpdate</term>
    ///     <description>서버로부터 데이터를 받을때 호출됩니다. 해당 구현을 통해 동기화된 데이터를 처리해 주세요.</description>
    /// </item>
    /// <item>
    ///     <term>sendSyncFixedUpdate</term>
    ///     <description>데이터를 서버로 전송할때 호출됩니다. sendSyncUpdate와 다른 점은 Unity의 FixedUpdate 이벤트 위에서 작동합니다. Physic적인 동기화에 사용할 수 있습니다.</description>
    /// </item>
    /// <item>
    ///     <term>receiveSyncFixedUpdate</term>
    ///     <description>서버로부터 데이터를 받을때 호출됩니다. receiveSyncUpdate와 다른 점은 Unity의 FixedUpdate 이벤트 위에서 작동합니다. Physic적인 동기화에 사용할 수 있습니다.</description>
    /// </item>
    /// </list>
    /// 각 이벤트 함수를 통해 데이터 동기화 시 행동을 구현할 수 있습니다.
    ///
    /// 
    /// <br/>
    /// 프로퍼티 동기화 예제는 다음과 같습니다.
    /// <code language="lua">
    /// -- 본 예제는 간단한 Transform Position 동기화 예제입니다.
    /// 
    /// --@details 서버 혹은 플레이어로부터 데이터를 처음 받아올때 호출됩니다.
    /// function initializeSync()
    ///     self.transform.position = Vector3(0, 0, 0)
    /// end
    ///
    /// --@details SyncView가 초기화 되어 네트워크와 연결되었을때 호출됩니다.
    /// function onSyncViewInitialized()
    ///     -- 이 시점 이후 부터 네트워크와 관련된 동작을 수행 할 수 있습니다. 
    /// end
    /// ---@details 오브젝트의 소유권이 내것일 때 동기화 하고 싶은 Table을 리턴하면 됩니다.
    /// function sendSyncUpdate()
    ///     -- 현재 transform position을 동기화하기위하여 테이블 리턴 
    ///     local syncParam = {self.transform.position.x, self.transform.position.y, self.transform.position.z}
    ///     return syncParam
    /// end
    /// ---@details 오브젝트가 내것이 아니면 동기화 받은 데이터 처리 로직을 작성 하시면 됩니다.
    /// function receiveSyncUpdate(syncTable)
    ///     if (syncTable == nil or #syncTable &lt; 3) then 
    ///         return 
    ///     end
    ///     -- 동기화 받은 데이터로 transform 업데이트
    ///     self.transform.position = Vector3(syncTable[1], syncTable[2], syncTable[3])
    /// end
    /// ---@details 소유권을 요청함
    /// function requestOwnership()
    ///     -- SyncView는 숏컷입니다.
    ///     SyncView:RequestOwnership()
    /// end
    /// 
    /// ---@return boolean
    /// ---@details 소유권이 나에게 있는지를 반환함
    /// function getIsMine()
    ///     return SyncView.IsMine
    /// end
    /// </code>
    /// </para>
    /// RPC는 Remote Procedure Call(원격 프로시저 호출)의 약자로, 다른 유저의 네트워크 객체 내의 메소드 함수를 원격으로 호출 할 수 있는 기능을 제공합니다.
    /// View와 달리 연속적인 데이터 동기화가 아니라, 단방향의 이벤트성 동기화를 위해 사용합니다.
    ///
    /// 예제는 다음과 같습니다.
    /// 
    /// <para>
    /// <code language="lua">
    /// -- 나를 포함한 모든 방의 유저들에게 보내는 경우
    /// function rpc1()
    ///     local AllOption = RPCSendOption.All
    /// 
    ///     -- 필요한 parameter는 다음과 같습니다.
    ///       -- 1. 해당 rpc 코드를 작성하고 있는 스크립트 이름
    ///       -- 2. 실행하도록 하고   싶은 메소드   이름
    ///       -- 3. RPC option
    ///       -- 4. 실행하려는 메소드의 parameter. 사용 가능한 parameter 개수는 최대 5개입니다. 
    ///     SyncView:SendRPC("SendAll", AllOption, nil)
    /// end
    /// 
    /// function SendAll() 
    ///     -- 이 메소드는 나를 포함한 모든 유저들이 실행하게 됩니다. 
    /// end
    /// -- 나를 제외한 모든 방의 유저들에게 보내는 경우
    /// function rpc2()
    ///     local OthersOption = RPCSendOption.Others
    ///     local table = {9, 1}
    /// 
    ///     SyncView:SendRPC("SendOthers", OthersOption, table)
    /// end
    /// 
    /// function SendOthers(a, b) 
    ///     -- 이       메소드는 나를 제외한 유저들이 실행하게 됩니다.
    ///     -- {9, 1}을 보냈으므로,  a = 9,   b = 1 입니다. 
    /// end
    /// </code>
    /// <b>SendTargetRPC</b>
    /// <br/>
    /// 유저의 id를 입력하여 해당 유저가 특정 메소드를 실행하도록 할 수 있습니다. 유저의 id는 Player.Other.GetPlayerID() 를 사용하여 알 수 있습니다.
    /// <code language="lua">
    /// function rpc3()
    ///     local players = {}
    ///     -- 닉네임이 "targetPlayerNickname"인 유저의 id를 받아옵니다.
    ///     players[1] = Player.Other.GetPlayerID("targetPlayerNickname")
    /// 
    ///     SyncView:SendTargetRPC("Test", "SendTarget", players, nil)
    /// end
    /// 
    ///     function SendTarget()
    ///     -- 닉네임이 "targetPlayerNickname"인 유저만 이 메소드를 실행하게 됩니다.
    ///     end 
    /// </code>
    /// 
    /// </para>
    /// </remarks>
    [AddComponentMenu("VivenSDK/Network/Viven Custom Sync View")]
    public class VivenCustomSyncView : MonoBehaviour
    {
        /// <summary>
        /// 동기화를 수행할 LuaBehaviour입니다.
        /// </summary>
        public VivenLuaBehaviour luaBehaviour;

        /// <summary>
        /// 동기화 방법에 대한 타입입니다.
        /// </summary>
        public SDKSyncType viewSyncType = SDKSyncType.Continuous;

        /// <summary>
        /// 해당 컴포넌트가 속한 VObject의 소유권을 요청합니다.
        /// </summary>
        public void RequestOwnership()
        {
        }


        /// <summary>
        /// 해당 컴포넌트가 속한 VObject의 소유권이 내것인지 확인합니다.
        /// </summary>
        /// <returns></returns>
        public bool IsMine { get; set; }

        /// <summary>
        /// SDK에서 RPC를 보냄. SendOption이 Target일 경우, TargetRPC를 사용해주세요
        /// </summary>
        /// <param name="functionName">호출할 함수 이름</param>
        /// <param name="option">RPC 전송 옵션</param>
        /// <param name="args">전달할 인자</param>
        public void SendRPC(string functionName, SDKRPCSendOption option, params object[] args)
        {
        }

        /// <summary>
        /// SDK에서 TargetRPC를 보냄
        /// </summary>
        /// <param name="functionName">호출할 함수 이름</param>
        /// <param name="playerIds">RPC를 보낼 플레이어의 ID</param>
        /// <param name="args">전달할 인자</param>
        public void SendTargetRPC(string functionName, string[] playerIds, params object[] args)
        {
        }
    }
}