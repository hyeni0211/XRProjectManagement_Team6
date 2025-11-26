using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Twoz.Viven.Interactions
{
    /// <summary>
    /// 오브젝트를 Grab중일 때 손의 Pose를 나타냅니다.
    /// </summary>
    /// <remarks>
    /// 오브젝트를 Grab하는 동안 손을 특정한 모양으로 설정할 수 있습니다.
    /// 예를 들어, 활 오브젝트를 Grab하는 동안 손은 활을 잡고 있도록 손의 Pose를 변경할 수 있습니다.
    /// HandPose는 왼손과 오른손을 각각 설정할 수 있습니다.
    /// <para>
    /// HandPose를 설정하지 않으면 기본 포즈를 사용합니다.
    /// </para>
    /// <para>
    /// HandPose는 손가락과 손목의 회전값으로 나타냅니다. 각 손가락의 회전값은 이전 관절에서의 Local Rotation으로 설정합니다.
    /// 각 관절의 Hierarchy는 Humanoid 아바타를 바탕으로 구성되어 있습니다.
    /// <list type="bullet">
    /// <item>Thumb
    ///     <list type="bullet">
    ///         <item>Proximal</item>
    ///         <item>Intermediate</item>
    ///         <item>Distal</item>
    ///     </list>
    /// </item>
    /// <item>Index
    ///     <list type="bullet">
    ///         <item>Proximal</item>
    ///         <item>Intermediate</item>
    ///         <item>Distal</item>
    ///     </list>
    /// </item>
    /// <item>Middle
    ///     <list type="bullet">
    ///         <item>Proximal</item>
    ///         <item>Intermediate</item>
    ///         <item>Distal</item>
    ///     </list>
    /// </item>
    /// <item>Ring
    ///     <list type="bullet">
    ///         <item>Proximal</item>
    ///         <item>Intermediate</item>
    ///         <item>Distal</item>
    ///     </list>
    /// </item>
    /// <item>little
    ///     <list type="bullet">
    ///         <item>Proximal</item>
    ///         <item>Intermediate</item>
    ///         <item>Distal</item>
    ///     </list>
    /// </item>
    /// <item>Wrist</item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable, CreateAssetMenu(fileName = " TwozHandPose", menuName = "Viven/Create GrabPose", order = 1)]
    public class VivenGrabHandPose : ScriptableObject
    {
        
        /// <value> 엄지의 Local Rotation </value>
        [FormerlySerializedAs("thumbFinger")] [SerializeField] public Quaternion[] thumb;
        /// <value> 검지의 Local Rotation </value>
        [FormerlySerializedAs("indexFinger")] [SerializeField] public Quaternion[] index;
        /// <value> 중지의 Local Rotation </value>
        [FormerlySerializedAs("middleFinger")] [SerializeField] public Quaternion[] middle;
        /// <value> 약지의 Local Rotation </value>
        [FormerlySerializedAs("ringFinger")] [SerializeField] public Quaternion[] ring;
        /// <value> 소지의 Local Rotation </value>
        [FormerlySerializedAs("littleFinger")] [SerializeField] public Quaternion[] little;
        
        /// <summary>
        /// 손목의 회전값을 나타냅니다.
        /// </summary>
        /// <value>손목의 회전값</value>
        [FormerlySerializedAs("wristRot")] [SerializeField] public Quaternion wrist;

        /// <summary>
        /// 왼손인지 오른손인지 구분합니다.
        /// </summary>
        /// <value>True라면 왼손입니다.</value>
        [FormerlySerializedAs("isLeftHand")] [SerializeField] public bool isLeft;
    }
}