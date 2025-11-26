using UnityEngine;
using UnityEngine.Serialization;

namespace Twoz.Viven.Interactions
{
    /// <summary>
    /// 해당 오브젝트에 앉을 수 있도록 할 때 사용합니다. 
    /// </summary>
    /// <remarks>
    /// SittableModule을 사용하여 캐릭터가 앉을 수 있는 오브젝트를 만들 수 있습니다.
    /// 앉기를 실행하면, 캐릭터가 해당 Transform으로 이동 후 앉는 모션을 취합니다. 앉은 캐릭터는 이동할 수 없고, Yaw 회전만 가능합니다.
    /// <para>
    /// 플레이어는 다른 Grabbable 오브젝트를 잡거나 상호작용할 수 있습니다.
    /// </para>
    /// <para>
    /// TwozSittable은 추후 변경될 예정입니다.
    /// </para>
    /// </remarks>
    [RequireComponent(typeof(VObject), typeof(Collider))]
    [AddComponentMenu("VivenSDK/Interaction/Viven Sittable")]
    public class VivenSittable : MonoBehaviour
    {
        /// <summary>
        /// 캐릭터가 앉을 위치. 앉기를 실행하면, 캐릭터가 해당 Transform으로 이동 후 앉는 모션을 취합니다.
        /// </summary>
        [FormerlySerializedAs("vivenSitPoint")] [Tooltip("캐릭터가 앉을 위치. 앉기를 실행하면, 캐릭터가 해당 Transform으로 이동 후 앉는 모션을 취합니다.")]
        public Transform sitPoint;
    }
}
