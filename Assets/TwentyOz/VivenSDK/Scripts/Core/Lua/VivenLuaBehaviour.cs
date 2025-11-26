using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TwentyOz.VivenSDK.Scripts.Core.Lua
{
    /// <summary>
    /// Lua스크립트 환경에 Injection 정보들을 추가합니다.
    /// </summary>
    /// <remarks>
    /// <para>지원하는 변수 타입:</para>
    /// <list type="bullet">
    ///     <item>Object - Unity Object 타입의 값</item>
    ///     <item>GameObject - GameObject 타입의 값</item>
    ///     <item>Vector3 - Vector3 타입의 값</item>
    ///     <item>float - 부동 소수점 값</item>
    ///     <item>int - 정수 값</item>
    ///     <item>bool - 불리언 값</item>
    ///     <item>string - 문자열 값</item>
    ///     <item>Color - Color 타입의 값</item>
    ///     <item>VivenScript - VivenScript 타입의 값</item>
    /// </list>
    /// <para>Unity 인스펙터에서 이름과 값을 설정하면, 해당 이름으로 Lua 스크립트 내에서 변수에 접근할 수 있습니다.</para>
    /// <example>
    /// <para>Unity에서 설정:</para>
    /// <code>
    /// // 인스펙터에서 injection.gameObjectValues에 "playerObject"라는 이름으로 플레이어 GameObject를 할당
    /// </code>
    /// <para>Lua 스크립트에서 사용:</para>
    /// <code>
    /// function start()
    ///     Debug.Log("Player name: " .. playerObject.name)
    /// end
    /// </code>
    /// </example>
    /// </remarks>
    [Serializable]
    public class Injection
    {
        public ObjectValue[]      objectValues;
        public GameObjectValue[]  gameObjectValues;
        public Vector3Value[]     vector3Values;
        public FloatValue[]       floatValue;
        public IntValue[]         intValue;
        public BoolValue[]        boolValue;
        public StringValue[]      stringValue;
        public ColorValue[]       colorValue;
        public VivenScriptValue[] vivenScriptValue;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class ObjectValue
    {
        public string name;
        public Object value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class GameObjectValue
    {
        public string     name;
        public GameObject value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class Vector3Value
    {
        public string  name;
        public Vector3 value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class FloatValue
    {
        public string name;
        public float  value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class IntValue
    {
        public string name;
        public int    value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class BoolValue
    {
        public string name;
        public bool   value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class StringValue
    {
        public string name;
        public string value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class ColorValue
    {
        public string name;
        public Color  value;
    }

    /// <summary>
    /// Viven Lua 스크립트에 주입할때 사용하는 구조체입니다.
    /// </summary>
    [Serializable]
    public class VivenScriptValue
    {
        public string      name;
        public VivenScript value;
    }

    /// <summary>
    /// VIVEN SDK에서 사용하는 LuaBehaviour
    /// Viven에서 컨텐츠를 제작하기 위해서 VivenLuaBehaviour를 사용합니다.
    /// </summary>
    /// <remarks>
    /// <para>VivenLuaBehaviour는 VIVEN SDK 내에서 Lua 스크립트를 실행하기 위한 기본 클래스입니다.</para>
    /// <para>이 클래스를 통해 Lua 스크립트에서 Unity의 기본 이벤트(Start, Update 등)와 VIVEN의 특수 이벤트(방 입장/퇴장, 사용자 입장/퇴장 등)를 처리할 수 있습니다.</para>
    /// <para>또한 다양한 유틸리티 메서드를 제공하여 Lua 스크립트에서 Unity 객체를 쉽게 조작할 수 있도록 합니다.</para>
    /// <example>
    /// <para>기본적인 사용 예시:</para>
    /// <code>
    /// -- Lua Script Example
    /// function start()
    ///     -- 스크립트가 시작될 때 실행되는 코드
    ///     Debug.Log("Script started!")
    /// end
    /// 
    /// function update()
    ///     -- 매 프레임마다 실행되는 코드
    /// end
    /// 
    /// function onRoomJoined(roomData)
    ///     -- 방에 입장했을 때 실행되는 코드
    ///     Debug.Log("Joined room: " .. roomData.roomId) 
    /// end
    /// </code>
    /// </example>
    /// <para> Lua 스크립트에서 등록 가능한 이벤트 목록 </para>
    /// 아래 함수들은 Lua 스크립트 안에서 정의할 수 있으며, Unity 엔진 및 VIVEN SDK 이벤트에 의해 호출됩니다.
    /// <code>
    /// -- Unity LifeCycle 이벤트
    /// function awake()                  -- Awake 시 호출
    /// function start()                  -- Start 시 호출
    /// function update()                 -- Update 매 프레임 호출
    /// function fixedUpdate()            -- FixedUpdate 매 프레임 호출
    /// function onDestroy()              -- Destroy 시 호출
    /// function onEnable()               -- OnEnable 시 호출
    /// function onDisable()              -- OnDisable 시 호출
    /// function onApplicationFocus(focusStatus: bool) -- 포커스 상태 변경 시 호출
    /// function onApplicationPause(pauseStatus: bool) -- 앱 일시정지/재개 시 호출
    /// function onApplicationQuit()      -- 애플리케이션 종료 시 호출
    ///
    /// -- Unity Collision/Trigger 이벤트
    /// function onCollisionEnter(collision)         -- 충돌 시작 (3D)
    /// function onCollisionEnter2D(collision)       -- 충돌 시작 (2D)
    /// function onCollisionExit(collision)          -- 충돌 종료 (3D)
    /// function onCollisionExit2D(collision)        -- 충돌 종료 (2D)
    /// function onCollisionStay(collision)          -- 충돌 유지 (3D)
    /// function onCollisionStay2D(collision)        -- 충돌 유지 (2D)
    /// function onTriggerEnter(collider)            -- 트리거 진입 (3D)
    /// function onTriggerEnter2D(collider)          -- 트리거 진입 (2D)
    /// function onTriggerExit(collider)             -- 트리거 종료 (3D)
    /// function onTriggerExit2D(collider)           -- 트리거 종료 (2D)
    /// function onTriggerStay(collider)             -- 트리거 안에 머무름 (3D)
    /// function onTriggerStay2D(collider)           -- 트리거 안에 머무름 (2D)
    ///
    /// -- VIVEN Network (DTS) 이벤트
    /// function onRoomJoined(roomData)        -- 방 입장 완료 시 호출
    /// function onRoomUserJoined(userData)     -- 다른 사용자가 방에 입장했을 때
    /// function onUserLeaveRoom(userData)      -- 다른 사용자가 방에서 나갔을 때
    /// function onRoomLeave()                  -- 자신이 방을 나갈 때
    /// function onRoomPropChanged(propId, propVal) -- 방 속성 변경 시 호출
    /// function onMapPropChanged(mapId, propId, propVal) -- 맵 속성 변경 시 호출
    ///
    /// -- VIVEN Player Trigger 이벤트
    /// function onPlayerEnter(userId)           -- 플레이어가 트리거에 들어올 때
    /// function onPlayerExit(userId)            -- 플레이어가 트리거에서 나갈 때
    /// function onPlayerStay(userId)            -- 플레이어가 트리거 안에 머무를 때
    ///
    /// -- Text Chat API 이벤트
    /// function onChannelTextMessageReceived(senderName, message) -- 채널 채팅 메시지 수신
    /// function onDirectTextMessageReceived(senderName, message)  -- 개인 채팅 메시지 수신
    ///
    /// -- VIVEN Grabbable/Attachable 객체 이벤트
    /// function onGrab()                   -- 객체를 잡았을 때
    /// function onRelease()                -- 객체를 놓았을 때
    /// function objectShortClickAction()   -- 객체를 짧게 클릭했을 때
    /// function objectLongClickAction()    -- 객체를 길게 클릭했을 때
    /// function objectHoldActionStart()    -- 객체를 눌렀을 때 시작
    /// function objectHoldActionEnd()      -- 객체를 누르다 떼었을 때
    ///
    /// -- Button1/2/3 별 추가 액션 (Grabbable)
    /// function ShortClickAction1()
    /// function LongClickAction1()
    /// function HoldActionStart1()
    /// function HoldActionEnd1()
    /// function ShortClickAction2()
    /// function LongClickAction2()
    /// function HoldActionStart2()
    /// function HoldActionEnd2()
    /// function ShortClickAction3()
    /// function LongClickAction3()
    /// function HoldActionStart3()
    /// function HoldActionEnd3()
    ///
    /// -- VIVEN Attach Point 이벤트
    /// function onAttach()                 -- 부착되었을 때
    /// function onDetach()                 -- 분리되었을 때
    /// </code>
    /// <para>
    /// ### Unity LifeCycle 이벤트 사용법
    /// </para>
    /// <remarks>
    /// <para>이를 통해 Lua 스크립트에서 다음과 같은 Unity LifeCycle 함수들을 정의하여 사용할 수 있습니다:</para>
    /// <list type="bullet">
    ///     <item>start() - Start 이벤트</item>
    ///     <item>update() - Update 이벤트</item>
    ///     <item>fixedUpdate() - FixedUpdate 이벤트</item>
    ///     <item>onDestroy() - OnDestroy 이벤트</item>
    ///     <item>onEnable() - OnEnable 이벤트</item>
    ///     <item>onDisable() - OnDisable 이벤트</item>
    ///     <item>onApplicationFocus(focusStatus) - OnApplicationFocus 이벤트</item>
    ///     <item>onApplicationPause(pauseStatus) - OnApplicationPause 이벤트</item>
    ///     <item>onApplicationQuit() - OnApplicationQuit 이벤트</item>
    ///     <item>onCollisionEnter(collision) - OnCollisionEnter 이벤트</item>
    ///     <item>onCollisionEnter2D(collision) - OnCollisionEnter2D 이벤트</item>
    ///     <item>onCollisionExit(collision) - OnCollisionExit 이벤트</item>
    ///     <item>onCollisionExit2D(collision) - OnCollisionExit2D 이벤트</item>
    ///     <item>onCollisionStay(collision) - OnCollisionStay 이벤트</item>
    ///     <item>onCollisionStay2D(collision) - OnCollisionStay2D 이벤트</item>
    ///     <item>onTriggerEnter(collider) - OnTriggerEnter 이벤트</item>
    ///     <item>onTriggerEnter2D(collider) - OnTriggerEnter2D 이벤트</item>
    ///     <item>onTriggerExit(collider) - OnTriggerExit 이벤트</item>
    ///     <item>onTriggerExit2D(collider) - OnTriggerExit2D 이벤트</item>
    ///     <item>onTriggerStay(collider) - OnTriggerStay 이벤트</item>
    ///     <item>onTriggerStay2D(collider) - OnTriggerStay2D 이벤트</item>
    ///     <item>onPlayerEnter(userID) - 플레이어가 트리거에 들어올 때 호출</item>
    ///     <item>onPlayerExit(userID) - 플레이어가 트리거에서 나갈 때 호출</item>
    ///     <item>onPlayerStay(userID) - 플레이어가 트리거 내에 머무를 때 호출</item>
    /// </list>
    /// <para>또한 마우스 이벤트도 지원합니다:</para>
    /// <list type="bullet">
    ///     <item>onMouseDown() - OnMouseDown 이벤트</item>
    ///     <item>onMouseDrag() - OnMouseDrag 이벤트</item>
    ///     <item>onMouseEnter() - OnMouseEnter 이벤트</item>
    ///     <item>onMouseExit() - OnMouseExit 이벤트</item>
    ///     <item>onMouseOver() - OnMouseOver 이벤트</item>
    ///     <item>onMouseUp() - OnMouseUp 이벤트</item>
    ///     <item>onMouseUpAsButton() - OnMouseUpAsButton 이벤트</item>
    /// </list>
    /// <example>
    /// Lua 스크립트 예시:
    /// <code>
    /// function start()
    ///     Debug.Log("Script started!")
    /// end
    /// 
    /// function update()
    ///     -- 매 프레임마다 실행
    /// end
    /// 
    /// function onTriggerEnter(collider)
    ///     Debug.Log("Trigger entered by: " .. collider.gameObject.name)
    /// end
    /// 
    /// function onPlayerEnter(userID)
    ///     Debug.Log("Player entered: " .. userID)
    /// end
    /// </code>
    /// </example>
    /// </remarks>
    /// 
    /// <para>
    /// ### Viven Network(DTS)의 이벤트 사용법 
    /// </para>
    /// <para>DTS(Data Transfer Service)는 VIVEN의 네트워크 통신 시스템으로, 방 입장/퇴장, 사용자 연결/해제 등의 이벤트를 처리합니다.</para>
    /// <para>Lua 스크립트에서 다음과 같은 함수들을 정의하여 네트워크 이벤트를 처리할 수 있습니다:</para>
    /// <list type="bullet">
    ///     <item>onRoomJoined(roomData) - 방에 입장했을 때 호출</item>
    ///     <item>onRoomUserJoined(userData) - 다른 사용자가 방에 입장했을 때 호출</item>
    ///     <item>onUserLeaveRoom(userData) - 다른 사용자가 방에서 나갈 때 호출</item>
    ///     <item>onRoomLeave() - 자신이 방에서 나갈 때 호출</item>
    ///     <item>onMapChanged(mapName) - 맵이 변경되었을 때 호출</item>
    ///     <item>onRoomPropChanged(propId, propVal) - 방 속성이 변경되었을 때 호출</item>
    ///     <item>onMapPropChanged(mapId, propId, propVal) - 맵 속성이 변경되었을 때 호출</item>
    /// </list>
    /// <example>
    /// Lua 스크립트 예시:
    /// <code>
    /// function onRoomJoined(roomData)
    ///     Debug.Log("Joined room: " .. roomData.roomId)
    ///     Debug.Log("Room name: " .. roomData.roomName)
    /// end
    /// 
    /// function onRoomUserJoined(userData)
    ///     Debug.Log("User joined: " .. userData.nickname)
    /// end
    /// 
    /// function onUserLeaveRoom(userData)
    ///     Debug.Log("User left: " .. userData.nickname)
    /// end
    /// </code>
    /// </example>
    /// <para>
    /// ### TextChat 이벤트 사용법 
    /// </para>
    /// <para>Lua 스크립트에서 다음 함수를 정의하면 해당 이벤트를 받을 수 있습니다:</para>
    /// <list type="bullet">
    ///     <item>onChannelTextMessageReceived(sender, message) - 채널 메시지 수신 시 호출</item>
    ///     <item>onDirectTextMessageReceived(sender, message) - 직접 메시지 수신 시 호출</item>
    /// </list>
    /// <example>
    /// Lua 스크립트에서 다음과 같이 정의합니다:
    /// <code>
    /// function onChannelTextMessageReceived(sender, message)
    ///     Debug.Log(sender .. "가 보낸 메시지: " .. message)
    /// end
    /// 
    /// function onDirectTextMessageReceived(sender, message)
    ///     Debug.Log(sender .. "님의 귓속말: " .. message)
    /// end
    /// </code>
    /// </example>
    /// <para>
    /// ### VObject 이벤트 사용법 
    /// </para>
    /// <para>VIVEN 컴포넌트는 다음과 같은 두 가지 주요 컴포넌트의 이벤트를 지원합니다:</para>
    /// <list type="bullet">
    ///     <item>VivenGrabbableModule: 객체를 잡고 상호작용하기 위한 컴포넌트</item>
    ///     <item>VivenAttachPoint: 객체를 특정 지점에 부착하기 위한 컴포넌트</item>
    /// </list>
    /// <para>Lua 스크립트에서 다음과 같은 함수들을 정의하여 컴포넌트 이벤트를 처리할 수 있습니다:</para>
    /// <list type="number">
    ///     <item>GrabbableModule 이벤트:
    ///         <list type="bullet">
    ///             <item>onGrab() - 객체를 잡았을 때 호출</item>
    ///             <item>onRelease() - 객체를 놓았을 때 호출</item>
    ///             <item>objectShortClickAction() - 객체를 짧게 클릭했을 때 호출 (Button 1)</item>
    ///             <item>objectLongClickAction() - 객체를 길게 클릭했을 때 호출 (Button 1)</item>
    ///             <item>objectHoldActionStart() - 객체를 누르기 시작했을 때 호출 (Button 1)</item>
    ///             <item>objectHoldActionEnd() - 객체 누르기를 종료했을 때 호출 (Button 1)</item>
    ///             <item>ShortClickAction1() - Button 1 짧게 클릭</item>
    ///             <item>LongClickAction1() - Button 1 길게 클릭</item>
    ///             <item>HoldActionStart1() - Button 1 누르기 시작</item>
    ///             <item>HoldActionEnd1() - Button 1 누르기 종료</item>
    ///             <item>ShortClickAction2() - Button 2 짧게 클릭</item>
    ///             <item>LongClickAction2() - Button 2 길게 클릭</item>
    ///             <item>HoldActionStart2() - Button 2 누르기 시작</item>
    ///             <item>HoldActionEnd2() - Button 2 누르기 종료</item>
    ///             <item>ShortClickAction3() - Button 3 짧게 클릭</item>
    ///             <item>LongClickAction3() - Button 3 길게 클릭</item>
    ///             <item>HoldActionStart3() - Button 3 누르기 시작</item>
    ///             <item>HoldActionEnd3() - Button 3 누르기 종료</item>
    ///         </list>
    ///     </item>
    ///     <item>AttachPoint 이벤트:
    ///         <list type="bullet">
    ///             <item>onAttach() - 객체가 부착점에 부착되었을 때 호출</item>
    ///             <item>onDetach() - 객체가 부착점에서 분리되었을 때 호출</item>
    ///         </list>
    ///     </item>
    /// </list>
    /// <example>
    /// Lua 스크립트 예시:
    /// <code>
    /// function onGrab()
    ///     Debug.Log("Object grabbed!")
    /// end
    /// 
    /// function onRelease()
    ///     Debug.Log("Object released!")
    /// end
    /// 
    /// function objectShortClickAction()
    ///     Debug.Log("Object clicked!")
    /// end
    /// 
    /// function onAttach()
    ///     Debug.Log("Object attached to attach point!")
    /// end
    /// </code>
    /// </example>
    /// </remarks> 
    public class VivenLuaBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 사용자의 Lua 스크립트
        /// </summary>
        /// <remarks>
        /// <para>이 속성은 Unity 인스펙터에서 설정할 수 있으며, 실행될 Lua 스크립트를 지정합니다.</para>
        /// <para>스크립트는 .lua 파일 형식이어야 하며, 해당 파일의 내용이 실행됩니다.</para>
        /// </remarks>
        public VivenScript luaScript;

        /// <summary>
        /// 사용자의 Lua 스크립트에서 사용할 변수들
        /// </summary>
        /// <remarks>
        /// <para>이 속성은 Unity 인스펙터에서 설정할 수 있으며, Lua 스크립트에 주입할 변수들을 정의합니다.</para>
        /// <para>다양한 타입(GameObject, Transform, Vector3, float, int, bool, string, Color 등)의 변수를 Lua 스크립트에 전달할 수 있습니다.</para>
        /// <para>지원하는 변수 타입:</para>
        /// <list type="bullet">
        ///     <item>Object - Unity Object 타입의 값</item>
        ///     <item>GameObject - GameObject 타입의 값</item>
        ///     <item>Vector3 - Vector3 타입의 값</item>
        ///     <item>float - 부동 소수점 값</item>
        ///     <item>int - 정수 값</item>
        ///     <item>bool - 불리언 값</item>
        ///     <item>string - 문자열 값</item>
        ///     <item>Color - Color 타입의 값</item>
        ///     <item>VivenScript - VivenScript 타입의 값</item>
        /// </list>
        /// <example>
        /// <para>Unity에서 변수를 설정한 후, Lua 스크립트에서 다음과 같이 사용할 수 있습니다:</para>
        /// <code>
        /// -- 인스펙터에서 "targetObject"라는 이름으로 GameObject를 주입한 경우
        /// function start()
        ///     Debug.Log("Target object name: " .. targetObject.name)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Injection injection;

        //TODO 현재 사용하지 않으니 숨김
        // public bool        shouldSync  = true;
        // public bool        isGrabbable = false;

    #region Utilities

    #region Move

        /// <summary>
        /// 객체를 지정된 위치로 이동시킵니다.
        /// </summary>
        /// <param name="dir">목표 위치 (절대 좌표)</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOMove를 사용하여 현재 객체를 지정된 위치로 부드럽게 이동시킵니다.</para>
        /// <para>위치 애니메이션을 쉽게 만들 수 있으며, 반환된 Tween 객체를 사용하여 애니메이션을 제어할 수 있습니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function start()
        ///     -- 객체를 (0, 2, 0) 위치로 2초 동안 이동
        ///     local tween = self:DoMove(Vector3(0, 2, 0), 2)
        ///     -- 애니메이션이 완료되면 호출될 함수 설정
        ///     self:OnTweenComplete(tween, function()
        ///         Debug.Log("이동 완료!")
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoMove(Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 지정된 위치로 이동시킵니다.
        /// </summary>
        /// <param name="target">이동시킬 대상 Transform</param>
        /// <param name="dir">목표 위치 (절대 좌표)</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOMove를 사용하여 지정된 대상 객체를 지정된 위치로 부드럽게 이동시킵니다.</para>
        /// <para>현재 객체가 아닌 다른 객체의 위치 애니메이션을 제어할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function moveTargetObject()
        ///     -- targetObject는 injection을 통해 주입된 GameObject
        ///     local tween = self:DoMoveTarget(targetObject.transform, Vector3(0, 3, 0), 1.5)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoMoveTarget(Transform target, Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 X축으로 이동시킵니다.
        /// </summary>
        /// <param name="x">목표 X 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOMoveX를 사용하여 현재 객체의 X 좌표만 변경하여 이동시킵니다.</para>
        /// <para>한 축으로만 이동하는 애니메이션이 필요할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function slideRight()
        ///     -- 객체를 X축으로 5 위치로 1초 동안 이동
        ///     self:DoMoveX(5, 1)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoMoveX(float x, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 X축으로 이동시킵니다.
        /// </summary>
        /// <param name="target">이동시킬 대상 Transform</param>
        /// <param name="x">목표 X 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOMoveX를 사용하여 지정된 대상 객체의 X 좌표만 변경하여 이동시킵니다.</para>
        /// </remarks>
        public Tween DoMoveXTarget(Transform target, float x, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 Y축으로 이동시킵니다.
        /// </summary>
        /// <param name="y">목표 Y 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOMoveY를 사용하여 현재 객체의 Y 좌표만 변경하여 이동시킵니다.</para>
        /// <para>높이 변경이 필요한 애니메이션에 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function jump()
        ///     -- 객체를 Y축으로 2 위치로 0.5초 동안 이동(점프)
        ///     local jumpTween = self:DoMoveY(2, 0.5)
        ///     -- 완료 후 원래 위치로 돌아오기
        ///     self:OnTweenComplete(jumpTween, function()
        ///         self:DoMoveY(0, 0.5)
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoMoveY(float y, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 Y축으로 이동시킵니다.
        /// </summary>
        /// <param name="target">이동시킬 대상 Transform</param>
        /// <param name="y">목표 Y 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveYTarget(Transform target, float y, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 Z축으로 이동시킵니다.
        /// </summary>
        /// <param name="z">목표 Z 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveZ(float z, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 Z축으로 이동시킵니다.
        /// </summary>
        /// <param name="target">이동시킬 대상 Transform</param>
        /// <param name="z">목표 Z 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveZTarget(Transform target, float z, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 로컬 좌표 기준으로 이동시킵니다.
        /// </summary>
        /// <param name="dir">목표 위치 (로컬 좌표)</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOLocalMove를 사용하여 현재 객체를 부모 기준의 로컬 좌표로 이동시킵니다.</para>
        /// <para>부모-자식 관계가 있는 객체를 다룰 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function moveInParent()
        ///     -- 객체를 부모 기준 로컬 좌표 (1, 0, 1)로 이동
        ///     self:DoMoveLocal(Vector3(1, 0, 1), 1)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoMoveLocal(Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 로컬 좌표 기준으로 이동시킵니다.
        /// </summary>
        /// <param name="target">이동시킬 대상 Transform</param>
        /// <param name="dir">목표 위치 (로컬 좌표)</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveLocalTarget(Transform target, Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 로컬 X축으로 이동시킵니다.
        /// </summary>
        /// <param name="x">목표 로컬 X 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveLocalX(float x, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 로컬 X축으로 이동시킵니다.
        /// </summary>
        /// <param name="target">이동시킬 대상 Transform</param>
        /// <param name="x">목표 로컬 X 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveLocalXTarget(Transform target, float x, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 로컬 Y축으로 이동시킵니다.
        /// </summary>
        /// <param name="y">목표 로컬 Y 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveLocalY(float y, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 로컬 Y축으로 이동시킵니다.
        /// </summary>
        /// <param name="target">이동시킬 대상 Transform</param>
        /// <param name="y">목표 로컬 Y 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveLocalYTarget(Transform target, float y, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 로컬 Z축으로 이동시킵니다.
        /// </summary>
        /// <param name="z">목표 로컬 Z 좌표</param>
        /// <param name="duration">이동 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveLocalZ(float z, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 로컬 Z축으로 이동시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="z">목표 로컬 Z 좌표</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoMoveLocalZTarget(Transform target, float z, float duration)
        {
            return null;
        }

    #endregion

    #region Rotate

        /// <summary>
        /// 객체를 지정된 각도로 회전시킵니다.
        /// </summary>
        /// <param name="dir">목표 회전 각도 (Vector3, 오일러 각)</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DORotate를 사용하여 현재 객체를 지정된 회전 각도로 부드럽게 회전시킵니다.</para>
        /// <para>회전 애니메이션을 쉽게 만들 수 있으며, 반환된 Tween 객체를 사용하여 애니메이션을 제어할 수 있습니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function start()
        ///     -- 객체를 Y축 기준으로 360도 회전
        ///     local tween = self:DoRotate(Vector3(0, 360, 0), 2)
        ///     -- 무한 반복 설정
        ///     self:SetLoops(tween, -1, LoopType.Restart)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoRotate(Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 지정된 각도로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="dir">목표 회전 각도 (Vector3, 오일러 각)</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DORotate를 사용하여 지정된 대상 객체를 지정된 회전 각도로 부드럽게 회전시킵니다.</para>
        /// <para>현재 객체가 아닌 다른 객체의 회전 애니메이션을 제어할 때 유용합니다.</para>
        /// </remarks>
        public Tween DoRotateTarget(Transform target, Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 X축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="x">목표 X축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DORotate를 사용하여 현재 객체의 X축 회전 각도만 변경하여 회전시킵니다.</para>
        /// <para>X축(좌우 회전축)을 기준으로만 회전하는 애니메이션이 필요할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function nodHead()
        ///     -- 객체를 X축으로 30도 회전했다가 다시 원래대로 돌아오는 고개 끄덕임 효과
        ///     local tween = self:DoRotateX(30, 0.5)
        ///     self:OnTweenComplete(tween, function()
        ///         self:DoRotateX(0, 0.5)
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoRotateX(float x, float duration)
        {
            return null;
        }


        /// <summary>
        /// 대상 객체를 X축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="x">목표 X축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateXTarget(Transform target, float x, float duration)
        {
            return null;
        }


        /// <summary>
        /// 객체를 Y축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="y">목표 Y축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DORotate를 사용하여 현재 객체의 Y축 회전 각도만 변경하여 회전시킵니다.</para>
        /// <para>Y축(상하 회전축)을 기준으로만 회전하는 애니메이션이 필요할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function turnAround()
        ///     -- 객체를 Y축으로 180도 회전시켜 반대 방향을 바라보게 함
        ///     self:DoRotateY(180, 1)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoRotateY(float y, float duration)
        {
            return null;
        }


        /// <summary>
        /// 대상 객체를 Y축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="y">목표 Y축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateYTarget(Transform target, float y, float duration)
        {
            return null;
        }


        /// <summary>
        /// 객체를 Z축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="z">목표 Z축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DORotate를 사용하여 현재 객체의 Z축 회전 각도만 변경하여 회전시킵니다.</para>
        /// <para>Z축(앞뒤 회전축)을 기준으로만 회전하는 애니메이션이 필요할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function tiltObject()
        ///     -- 객체를 Z축으로 15도 기울임
        ///     self:DoRotateZ(15, 0.5)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoRotateZ(float z, float duration)
        {
            return null;
        }


        /// <summary>
        /// 대상 객체를 Z축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="z">목표 Z축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateZTarget(Transform target, float z, float duration)
        {
            return null;
        }


        /// <summary>
        /// 객체를 로컬 회전 좌표 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="dir">목표 로컬 회전 각도 (Vector3, 오일러 각)</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOLocalRotate를 사용하여 현재 객체를 부모 기준의 로컬 회전 좌표로 회전시킵니다.</para>
        /// <para>부모-자식 관계가 있는 객체의 회전을 다룰 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function rotateInParent()
        ///     -- 객체를 부모 기준 로컬 회전 좌표 (0, 90, 0)으로 회전
        ///     self:DoRotateLocal(Vector3(0, 90, 0), 1)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoRotateLocal(Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체를 로컬 회전 좌표 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="dir">목표 로컬 회전 각도 (Vector3, 오일러 각)</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateLocalTarget(Transform target, Vector3 dir, float duration)
        {
            return null;
        }


        /// <summary>
        /// 객체를 로컬 X축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="x">목표 로컬 X축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateLocalX(float x, float duration)
        {
            return null;
        }


        /// <summary>
        /// 대상 객체를 로컬 X축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="x">목표 로컬 X축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateLocalXTarget(Transform target, float x, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체를 로컬 Y축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="y">목표 로컬 Y축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateLocalY(float y, float duration)
        {
            return null;
        }


        /// <summary>
        /// 대상 객체를 로컬 Y축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="y">목표 로컬 Y축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateLocalYTarget(Transform target, float y, float duration)
        {
            return null;
        }


        /// <summary>
        /// 객체를 로컬 Z축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="z">목표 로컬 Z축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateLocalZ(float z, float duration)
        {
            return null;
        }


        /// <summary>
        /// 대상 객체를 로컬 Z축 기준으로 회전시킵니다.
        /// </summary>
        /// <param name="target">회전시킬 대상 Transform</param>
        /// <param name="z">목표 로컬 Z축 회전 각도</param>
        /// <param name="duration">회전 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoRotateLocalZTarget(Transform target, float z, float duration)
        {
            return null;
        }

    #endregion

    #region Scale

        /// <summary>
        /// 객체의 크기를 조절합니다.
        /// </summary>
        /// <param name="dir">목표 크기 (Vector3)</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOScale을 사용하여 현재 객체의 크기를 부드럽게 조절합니다.</para>
        /// <para>확대/축소 애니메이션을 쉽게 만들 수 있으며, 반환된 Tween 객체를 사용하여 애니메이션을 제어할 수 있습니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function pulseEffect()
        ///     -- 객체를 원래 크기의 1.2배로 확대했다가 다시 원래 크기로 돌아오는 효과
        ///     local tween = self:DoScale(Vector3(1.2, 1.2, 1.2), 0.5)
        ///     self:OnTweenComplete(tween, function()
        ///         self:DoScale(Vector3(1, 1, 1), 0.5)
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoScale(Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체의 크기를 조절합니다.
        /// </summary>
        /// <param name="target">크기를 조절할 대상 Transform</param>
        /// <param name="dir">목표 크기 (Vector3)</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOScale을 사용하여 지정된 대상 객체의 크기를 부드럽게 조절합니다.</para>
        /// <para>현재 객체가 아닌 다른 객체의 크기 애니메이션을 제어할 때 유용합니다.</para>
        /// </remarks>
        public Tween DoScaleTarget(Transform target, Vector3 dir, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체의 X축 크기를 조절합니다.
        /// </summary>
        /// <param name="x">목표 X축 크기</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOScaleX를 사용하여 현재 객체의 X축 크기만 변경합니다.</para>
        /// <para>가로 방향으로만 크기를 조절할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function stretchHorizontally()
        ///     -- 객체를 X축으로 2배 늘림
        ///     self:DoScaleX(2, 0.5)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoScaleX(float x, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체의 X축 크기를 조절합니다.
        /// </summary>
        /// <param name="target">크기를 조절할 대상 Transform</param>
        /// <param name="x">목표 X축 크기</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoScaleXTarget(Transform target, float x, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체의 Y축 크기를 조절합니다.
        /// </summary>
        /// <param name="y">목표 Y축 크기</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOScaleY를 사용하여 현재 객체의 Y축 크기만 변경합니다.</para>
        /// <para>세로 방향으로만 크기를 조절할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function stretchVertically()
        ///     -- 객체를 Y축으로 2배 늘림
        ///     self:DoScaleY(2, 0.5)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween DoScaleY(float y, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체의 Y축 크기를 조절합니다.
        /// </summary>
        /// <param name="target">크기를 조절할 대상 Transform</param>
        /// <param name="y">목표 Y축 크기</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoScaleYTarget(Transform target, float y, float duration)
        {
            return null;
        }

        /// <summary>
        /// 객체의 Z축 크기를 조절합니다.
        /// </summary>
        /// <param name="z">목표 Z축 크기</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 라이브러리의 DOScaleZ를 사용하여 현재 객체의 Z축 크기만 변경합니다.</para>
        /// <para>깊이 방향으로만 크기를 조절할 때 유용합니다.</para>
        /// </remarks>
        public Tween DoScaleZ(float z, float duration)
        {
            return null;
        }

        /// <summary>
        /// 대상 객체의 Z축 크기를 조절합니다.
        /// </summary>
        /// <param name="target">크기를 조절할 대상 Transform</param>
        /// <param name="z">목표 Z축 크기</param>
        /// <param name="duration">변환 시간(초)</param>
        /// <returns>생성된 Tween 객체</returns>
        public Tween DoScaleZTarget(Transform target, float z, float duration)
        {
            return null;
        }

    #endregion

    #region Callbacks

        /// <summary>
        /// Tween 애니메이션을 지정된 시간만큼 지연시킵니다.
        /// </summary>
        /// <param name="tween">지연시킬 Tween 객체</param>
        /// <param name="delay">지연 시간(초)</param>
        /// <remarks>
        /// <para>이 메서드는 Tween 애니메이션의 시작을 지정된 시간만큼 지연시킵니다.</para>
        /// <para>여러 애니메이션을 순차적으로 실행하거나 타이밍을 조절할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function start()
        ///     -- 객체를 위로 이동하는 애니메이션을 1초 지연 후 시작
        ///     local tween = self:DoMoveY(2, 1)
        ///     self:DelayTween(tween, 1)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public void DelayTween(Tween tween, float delay)
        {
        }

        /// <summary>
        /// Tween 애니메이션이 완료되었을 때 실행될 콜백을 설정합니다.
        /// </summary>
        /// <param name="tween">콜백을 설정할 Tween 객체</param>
        /// <param name="action">애니메이션 완료 시 실행될 Action</param>
        /// <remarks>
        /// <para>이 메서드는 Tween 애니메이션이 완료되었을 때 실행될 콜백 함수를 설정합니다.</para>
        public void OnTweenComplete(Tween tween, Action action)
        {
        }

        public void OnTweenKill(Tween tween, Action action)
        {
        }

        public void OnTweenStart(Tween tween, Action action)
        {
        }

        public void OnTweenUpdate(Tween tween, Action action)
        {
        }

        public void OnTweenStepComplete(Tween tween, Action action)
        {
        }

        public void OnTweenRewind(Tween tween, Action action)
        {
        }

        public void OnTweenPlay(Tween tween, Action action)
        {
        }

        public void OnTweenPause(Tween tween, Action action)
        {
        }

        public void OnTweenWaypointChange(Tween tween, Action<int> action)
        {
        }

    #endregion

    #region ETC

        public Tween DoShake(float duration,         float strength = 1f, int vibrato = 10, float randomness = 90f,
            bool                   snapping = false, bool  fadeOut  = true)
        {
            return null;
        }

        public Tween DoPunchScale(float duration)
        {
            return null;
        }


        public Tween PlayTween(Tween tween)
        {
            return null;
        }

        public Tween PauseTween(Tween tween)
        {
            return null;
        }

        public void KillTween(Tween tween, bool complete = false)
        {
        }

        public void RewindTween(Tween tween)
        {
        }

        public void RestartTween(Tween tween)
        {
        }

        public void CompleteTween(Tween tween)
        {
        }

        public void FlipTween(Tween tween)
        {
        }

        public void GotoTween(Tween tween, float to, bool andPlay = false)
        {
        }

        public void TogglePauseTween(Tween tween)
        {
        }

        public void SmoothRewindTween(Tween tween)
        {
        }

        public void RestartTweenAll(bool includeDelayed = true)
        {
        }

    #endregion

    #endregion

    #region OnTweenEvent

        /// <summary>
        /// Tween이 시작될 때 실행될 콜백 함수를 등록합니다.
        /// </summary>
        /// <param name="tween">콜백을 등록할 Tween 객체</param>
        /// <param name="callbackFunc">Tween이 시작될 때 실행될 함수</param>
        /// <returns>콜백이 등록된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 애니메이션이 시작될 때 지정된 Lua 콜백 함수를 실행합니다.</para>
        /// <para>애니메이션 시작 시점에 효과음 재생, 이벤트 트리거 등의 작업을 수행할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function animateObject()
        ///     local tween = self:DoMove(Vector3(0, 2, 0), 1)
        ///     self:OnTweenStart(tween, function()
        ///         print("애니메이션 시작!")
        ///         -- 여기에 애니메이션 시작 시 실행할 코드를 작성
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween OnTweenStart(Tween tween, LuaFunction callbackFunc)
        {
            return null;
        }

        /// <summary>
        /// Tween이 매 프레임 업데이트될 때마다 실행될 콜백 함수를 등록합니다.
        /// </summary>
        /// <param name="tween">콜백을 등록할 Tween 객체</param>
        /// <param name="callbackFunc">Tween이 업데이트될 때마다 실행될 함수</param>
        /// <returns>콜백이 등록된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 애니메이션이 실행되는 동안 매 프레임마다 지정된 Lua 콜백 함수를 호출합니다.</para>
        /// <para>애니메이션의 진행 상태에 따라 다른 요소를 동기화하거나, 진행 상황을 UI에 표시할 때 유용합니다.</para>
        /// <para>주의: 이 콜백은 매 프레임마다 호출되므로 성능에 영향을 줄 수 있습니다. 필요한 경우에만 사용하세요.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function moveWithProgress()
        ///     local tween = self:DoMove(Vector3(0, 5, 0), 2)
        ///     self:OnTweenUpdate(tween, function()
        ///         -- 현재 진행 중인 애니메이션의 위치에 따라 다른 작업 수행
        ///         local currentHeight = self.transform.position.y
        ///         print("현재 높이: " .. currentHeight)
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween OnTweenUpdate(Tween tween, LuaFunction callbackFunc)
        {
            return null;
        }

        /// <summary>
        /// Tween이 완료된 후 실행될 콜백 함수를 등록합니다.
        /// </summary>
        /// <param name="tween">콜백을 등록할 Tween 객체</param>
        /// <param name="callbackFunc">Tween이 완료된 후 실행될 함수</param>
        /// <returns>콜백이 등록된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 애니메이션이 모두 완료된 후 지정된 Lua 콜백 함수를 실행합니다.</para>
        /// <para>애니메이션 완료 후 다음 애니메이션을 시작하거나, 특정 이벤트를 트리거하는 등의 작업에 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function sequentialAnimation()
        ///     -- 첫 번째 애니메이션
        ///     local firstTween = self:DoMove(Vector3(0, 2, 0), 1)
        ///     
        ///     -- 첫 번째 애니메이션이 끝나면 두 번째 애니메이션 실행
        ///     self:OnTweenComplete(firstTween, function()
        ///         print("첫 번째 애니메이션 완료!")
        ///         self:DoRotate(Vector3(0, 180, 0), 0.5)
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween OnTweenComplete(Tween tween, LuaFunction callbackFunc)
        {
            return null;
        }

        /// <summary>
        /// Tween이 스텝(특정 진행률)에 도달할 때마다 실행될 콜백 함수를 등록합니다.
        /// </summary>
        /// <param name="tween">콜백을 등록할 Tween 객체</param>
        /// <param name="stepIncrement">콜백을 호출할 진행률 간격 (0.1 = 10% 마다)</param>
        /// <param name="callbackFunc">각 스텝에서 호출될 함수</param>
        /// <returns>콜백이 등록된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DoTween 애니메이션의 진행률이 지정된 간격에 도달할 때마다 콜백 함수를 호출합니다.</para>
        /// <para>애니메이션 진행 중 특정 단계마다 이벤트를 트리거하거나 중간 상태를 확인할 때 유용합니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function moveWithSteps()
        ///     local tween = self:DoMove(Vector3(0, 10, 0), 5)
        ///     -- 20%마다 콜백 함수 호출
        ///     self:OnTweenStep(tween, 0.2, function()
        ///         print("애니메이션 20% 진행됨!")
        ///         -- 여기에 특정 진행률에서 실행할 코드 작성
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween OnTweenStep(Tween tween, float stepIncrement, LuaFunction callbackFunc)
        {
            return null;
        }

        /// <summary>
        /// Tween이 웨이포인트에 도달할 때마다 실행될 콜백 함수를 등록합니다.
        /// </summary>
        /// <param name="tween">콜백을 등록할 Tween 객체</param>
        /// <param name="callbackFunc">웨이포인트에 도달할 때마다 호출될 함수(웨이포인트 인덱스를 매개변수로 받음)</param>
        /// <returns>콜백이 등록된 Tween 객체</returns>
        /// <remarks>
        /// <para>이 메서드는 DOTween의 경로 애니메이션(Path)에서 각 웨이포인트에 도달할 때마다 지정된 Lua 콜백 함수를 호출합니다.</para>
        /// <para>경로를 따라 이동하는 애니메이션에서 특정 지점마다 이벤트를 트리거하거나 상태를 변경할 때 유용합니다.</para>
        /// <para>콜백 함수는 현재 도달한 웨이포인트의 인덱스를 매개변수로 받습니다.</para>
        /// <example>
        /// <para>Lua 스크립트 예시:</para>
        /// <code>
        /// function moveAlongPath()
        ///     -- 경로 웨이포인트 정의
        ///     local path = {Vector3(0, 0, 0), Vector3(2, 3, 0), Vector3(4, 1, 0), Vector3(6, 2, 0)}
        ///     
        ///     -- 경로를 따라 이동하는 애니메이션 생성
        ///     local pathTween = VivenTweenUtil.DoPath(self.transform, path, 5, Ease.Linear)
        ///     
        ///     -- 각 웨이포인트에 도달할 때마다 콜백 함수 호출
        ///     self:OnTweenWaypoint(pathTween, function(waypointIndex)
        ///         print("웨이포인트 " .. waypointIndex .. "에 도달!")
        ///         
        ///         -- 특정 웨이포인트에서 추가 동작 수행
        ///         if waypointIndex == 2 then
        ///             -- 중간 지점에서 특별한 효과 재생
        ///             print("중간 지점 도달! 특수 효과 재생")
        ///         end
        ///     end)
        /// end
        /// </code>
        /// </example>
        /// </remarks>
        public Tween OnTweenWaypoint(Tween tween, LuaFunction callbackFunc)
        {
            return null;
        }

    #endregion
    }

    /// <summary>
    /// LuaFunction은 Lua 스크립트에서 호출할 수 있는 함수를 나타냅니다.
    /// </summary>
    public class LuaFunction
    {
    }

    /// <summary>
    /// Tween은 DoTween 애니메이션을 나타내는 객체입니다.
    /// </summary>
    public class Tween
    {
    }
}