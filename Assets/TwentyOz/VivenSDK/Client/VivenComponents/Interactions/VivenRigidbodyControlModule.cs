using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields;
using UnityEngine;
using UnityEngine.Serialization;

namespace Twoz.Viven.Interactions
{
    /// <summary>
    /// Viven에서 사용할 오브젝트의 Rigidbody를 제어하는 컴포넌트입니다. RigidBody에 직접 접근해서 사용하는 것은 권장되지 않습니다. 
    /// </summary>
    /// <remarks>
    /// Viven에서는 Interaction, Network, Physics 등의 기능을 제공하기 위해 Rigidbody를 제어하는 컴포넌트를 제공합니다.
    /// RigidBody에 직접 접근할 경우 동기화, 상호작용에 문제가 발생할 수 있습니다. RigidbodyControlModule을 사용해 Rigidbody를 제어하십시오.
    ///
    /// <para>
    /// <see cref="physicsType"/>은 물리적 상호작용을 활성화할 지 여부를 결정합니다. <see cref="SDKPhysicsType.Kinematic"/>인 경우 물리적 상호작용이 비활성화됩니다.
    /// <see cref="SDKPhysicsType.Physics"/> 인 경우 물리적 상호작용이 활성화됩니다.
    /// </para>
    /// 
    /// <para>
    /// RigidbodyControlModule에서 설정한 값들은 VR 상호작용에 영향을 미칩니다.
    /// 물체의 무게, Drag, AngularDrag, Center Of Mass 등을 설정해 VR 환경에서 물리적 상태를 조절할 수 있습니다.
    /// 물리적 상호작용을 비활성화하려면 physicsType을 Kinematic으로 설정하십시오.
    /// </para>
    ///
    /// <para>
    /// RigidbodyControlModule은 다음과 같은 기능을 제공합니다.
    /// <br/><u>2024.12.11 : API 기능 추가중에 있습니다...</u>
    /// <list type="bullet">
    /// 
    /// </list>
    /// 
    /// </para>
    /// </remarks>
    [AddComponentMenu("VivenSDK/Network/Viven Rigidbody Control Module")]
    public class VivenRigidbodyControlModule : MonoBehaviour
    {
        [FormerlySerializedAs("objectPhysicsType")] [SerializeField] public SDKPhysicsType physicsType;
        
        /// <summary>
        /// <see cref="Rigidbody"/>의 mass 입니다.
        /// </summary>
        [Header("Rigidbody Fields")]
        [SerializeField] public float originMass = 1f;
        
        /// <summary>
        /// <see cref="Rigidbody"/>의 Drag 입니다.
        /// </summary>
        [SerializeField] public float originDrag = 0f;
        
        /// <summary>
        /// <see cref="Rigidbody"/>의 AngularDrag 입니다.
        /// </summary>
        [SerializeField] public float originAngularDrag = 0.05f;
        
        /// <summary>
        /// Center Of Mass를 자동으로 계산할지 여부입니다.
        /// 해당 값이 False일 경우, originCom 값을 사용합니다.
        /// </summary>
        [SerializeField] public bool automaticCenterOfMass = true;
        
        /// <summary>
        /// <see cref="Rigidbody"/>의 Center Of Mass 입니다.
        /// </summary>
        [SerializeField] public Vector3 originCom;
    }
}