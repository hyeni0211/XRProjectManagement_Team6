using System;
using System.Collections.Generic;
using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields;
using Twoz.Viven.Interactions.Interactor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Twoz.Viven.Interactions
{
    /// <summary>
    /// 플레이어가 상호작용하기 위한 기본적인 기능을 제공합니다.
    /// </summary>
    /// <remarks>
    /// 플레이어가 손에 잡고 사용하는 오브젝트에는 GrabbableModule이 추가되어야 합니다.
    /// <para>
    ///     GrabbableModule들은 물리적 상태가 네트워크를 통해 동기화됩니다.
    ///     <br/>이를 위해 해당 오브젝트는 RigidbodyControlModule을 포함해야 합니다.
    ///     <br/>RigidBody가 아닌 오브젝트 고유의 상태, 변수를 동기화하기 위해서는 VivenBehaviour에서 RPC를 사용해 동기화를 수행해야 합니다.
    /// </para>
    /// <para>
    ///     GrabbableModule은 다음과 같은 기능을 제공합니다.
    ///     <list type="bullet">
    ///         <item>Grab 모드</item>
    ///         <item>배치 모드</item>
    ///     </list>
    ///     Grabbable Event 목록 
    ///     <br/>Grab모드는 물체와 플레이어가 상호작용 할 수 있는 모드입니다.<br/>다음 이벤트를 VivenBehaviour에서 정의해 기능을 설정할 수 있습니다.
    ///     <para>
    ///         <list type="bullet">
    ///             <item>
    ///                 <term><see cref="onGrabEvent"/></term>
    ///                 <description>플레이어가 오브젝트를 잡았을 때 발생하는 이벤트입니다.</description>
    ///             </item>
    ///             <item>
    ///                 <term><see cref="objectShortClickAction"/></term>
    ///                 <description>물체를 잡고 있는 채로 짧은 클릭을 하였을 때 발동합니다.</description>
    ///             </item>
    ///             <item>
    ///                 <term><see cref="objectLongClickAction"/></term>
    ///                 <description>물체를 잡고 있는 채로 길게 클릭을 하고 뗐을 때 발동합니다. 길게 클릭의 기준은 1초 입니다.</description>
    ///             </item>
    ///             <item>
    ///                 <term><see cref="objectHoldActionStart"/></term>
    ///                 <description>물체를 잡고 있는 채로 길게 클릭을 유지할 때 발동합니다.</description>
    ///             </item>
    ///             <item>
    ///                 <term><see cref="objectHoldActionEnd"/></term>
    ///                 <description>물체를 잡고 있는 채로 길게 클릭을 유지하다 뗐을 때 발동합니다.</description>
    ///             </item>
    ///         `</list>
    ///         HoldAction과 ClickAction을 같이 사용할 경우 실행 순서를 보장할 수 없습니다.
    ///         두 이벤트 종류를 같이 사용하는 것은 권장되지 않습니다.
    ///     </para>
    /// </para>
    /// </remarks>
    [RequireComponent(typeof(VObject), typeof(VivenGrabbableRigidView), typeof(VivenRigidbodyControlModule))]
    [AddComponentMenu("VivenSDK/Interaction/Viven Grabbable Module")]
    public class VivenGrabbableModule : MonoBehaviour
    {
        /// <summary>
        /// 오브젝트의 Grab Type을 설정합니다.
        /// </summary>
        /// <value>
        /// 오브젝트를 잡았을 시 물체의 움직임을 결정합니다.
        /// </value>
        [FormerlySerializedAs("objectGrabType")] [Tooltip("해당 오브젝트의 Grab Type을 설정합니다.")] [SerializeField]
        public SDKGrabType grabType;

        /// <summary>
        /// 오브젝트를 잡았을 때 GameObject의 부모를 변경할 지 결정합니다.
        /// </summary>
        /// <value>
        /// True이면, 오브젝트를 잡았을 때 오브젝트의 부모가 interactor으로 변경됩니다.
        /// </value>
        [Tooltip("해당 값이 True이면 오브젝트를 잡았을 때 해당 오브젝트의 부모가 잡은 손으로 변경됩니다.")] [SerializeField]
        protected bool parentToHandOnGrab = true;

        // /// <summary>
        // /// PC 모드에서 잡혔을 때의 포인트들
        // /// </summary>
        // [Tooltip("PC 모드에서 잡혔을 때의 포인트들")]
        // public List<SDKGrabPoint> grabPointsPC = new List<SDKGrabPoint>();
        //
        // /// <summary>
        // /// XR 모드에서 잡혔을 때의 포인트들
        // /// </summary>
        // [Tooltip("XR 모드에서 잡혔을 때의 포인트들")]
        // public List<SDKGrabPoint> grabPointsXR = new List<SDKGrabPoint>();


        /// <summary>
        /// 물체를 잡을 때, 물체가 잡힐 위치를 커스텀하게 결정할 수 있습니다. grab point가 없으면 잡았을 때의 위치로 잡힙니다.
        /// </summary>
        /// <value>
        /// 잡았을 때 오브젝트가 손에 붙는 local 위치
        /// </value>
        [FormerlySerializedAs("vivenGrabPointsPC")] [Tooltip("잡았을 때 오브젝트가 손에 붙는 local 위치")] [SerializeField]
        public List<VivenGrabPoint> grabPoints = new List<VivenGrabPoint>();

        /// <value>
        /// 길게 클릭의 기준 (초 단위), <c>holdTimeThreshold</c>보다 길게 누르면 <see cref="objectHoldActionStart"/>이벤트가 발생합니다.
        /// </value>
        [Tooltip("얼마나 길게 눌러야 길게 누른 것으로 인식할지에 대한 시간 (초 단위)")] [SerializeField]
        public float holdTimeThreshold = 1.0f;

        /// <value>
        /// 오브젝트를 던지는 세기
        /// </value>
        /// <remarks>
        /// <see cref="VivenRigidbodyControlModule"/>의 <c>physicsType</c>이 <see cref="SDKPhysicsType.Physics"/>일 때만 사용됩니다.
        /// </remarks>
        [FormerlySerializedAs("vivenThrowForce")] [Tooltip("오브젝트를 던지는 세기")] [SerializeField]
        public float throwForce = 5f;

        /// <value>
        /// 배치 모드에서 다른 오브젝트들이 붙을 수 있는 위치들
        /// </value>
        [FormerlySerializedAs("vivenAttachPoints")] [Tooltip("다른 오브젝트를 붙일 수 있는 attchPoint들")] [SerializeField]
        public List<VivenAttachPoint> attachPoints;

        // [NonSerialized] public List<SDKAttachPivot> vivenAttachPivots;

        /// <summary>
        /// Layer 설정을 유지해야 하는 게임 오브젝트
        /// </summary>
        /// <value>
        /// grabbable로 Layer를 변경하지 않을 게임 오브젝트
        /// </value>
        /// <remarks>
        /// <c>excludeLayerObjects</c>에 포함되지 않은 게임오브젝트들은 상호작용 과정에서 Layer가 변경됩니다. UI와 같은 Layer 설정을 유지해야 하는 오브젝트는 <c>excludeLayerObjects</c>에 추가해야 합니다.
        /// </remarks>
        [FormerlySerializedAs("excludeLayerGameObjects")] [Tooltip("Layer 설정을 유지해야 하는 게임 오브젝트")] [SerializeField]
        public List<GameObject> excludeLayerObjects = new List<GameObject>();

        /// <summary>
        /// Grab시 이벤트
        /// </summary>
        /// <value><c>onGrabEvent</c>는 물체를 잡은 순간 호출됩니다.</value>
        [NonSerialized] public UnityEvent onGrabEvent;

        /// <summary>
        /// Release시 이벤트
        /// </summary>
        /// <value><c>onReleaseEvent</c>는 물체를 놓은 순간 호출됩니다.</value>
        [NonSerialized] public UnityEvent onReleaseEvent;

        /// <summary>
        /// Grab 모드에서 짧게 눌렀다 땔 때 발생하는 이벤트
        /// </summary>
        /// <value><c>objectShortClickAction</c>은 물체를 잡은 상태에서 짧게 눌렀다 땔 때 호출됩니다.</value>
        [NonSerialized] public UnityEvent objectShortClickAction;

        /// <summary>
        /// Grab 모드에서 길게 눌렀다가 땔 때 발생하는 이벤트
        /// </summary>
        /// <value><c>objectLongClickAction</c>은 물체를 잡은 상태에서 길게 눌렀다가 땔 때 호출됩니다.</value>
        [NonSerialized] public UnityEvent objectLongClickAction;

        /// <summary>
        /// Grab 모드에서 길게 누르는 것이 인식되는 순간 발생하는 이벤트
        /// </summary>
        /// <value><c>objectHoldActionStart</c>은 물체를 잡은 상태에서 길게 누르는 것이 인식되는 순간 호출됩니다.(<see cref="holdTimeThreshold"/>초 이후 호출)</value>
        [NonSerialized] public UnityEvent objectHoldActionStart;

        /// <summary>
        /// <see cref="objectHoldActionStart"/>이벤트가 불린 후 액션 버튼을 뗐을 때 발생하는 이벤트
        /// </summary>
        /// <value><c>objectHoldActionEnd</c>은 <see cref="objectHoldActionStart"/>이벤트가 불린 후 버튼을 땔 때 호출됩니다.</value>
        [NonSerialized] public UnityEvent objectHoldActionEnd;
    }
}