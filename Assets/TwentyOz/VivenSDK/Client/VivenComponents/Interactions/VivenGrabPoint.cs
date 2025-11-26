using UnityEngine;
using UnityEngine.Serialization;

namespace Twoz.Viven.Interactions
{
    /// <summary>
    /// Grabbable 오브젝트를 잡을 때 손의 위치를 지정하는 컴포넌트입니다.
    /// </summary>
    /// <remarks>
    /// Grab 시 손의 모양은 <see cref="VivenGrabHandPose"/>를 통해 설정할 수 있습니다.
    /// </remarks>
    [AddComponentMenu("VivenSDK/Interaction/Viven Grab Point")]
    public class VivenGrabPoint : MonoBehaviour
    {
        /// <summary> Grab 시 손의 모양(회전값)을 지정 </summary>
        /// <value> Grab 시 손의 모양 </value>
        [FormerlySerializedAs("vivenGrabPose")] [SerializeField] public VivenGrabHandPose grabPose;
        /// <summary>
        /// 왼손인지 오른손인지 구분합니다.
        /// </summary>
        /// <value>True라면 왼손입니다.</value>
        [FormerlySerializedAs("isLeftHand")] public bool isLeft;
        
        /// <summary>
        /// XR 모드에서도 Grab Point를 적용할 것인지. true일 경우 XR 모드에서도 Grab Point를 적용합니다. false일 경우 XR 모드에서는 적용하지 않습니다.
        /// </summary>
        public bool isApplyInXR;
        
        /// <summary>
        /// TwozGrabPoint를 사용할 GrabbableModule을 부모 오브젝트들 중에서 찾습니다.
        /// </summary>
        /// <value>TwozGrabPoint를 사용할 GrabbableModule</value>
        public VivenGrabbableModule Gb => GetComponentInParent<VivenGrabbableModule>();
    }
}