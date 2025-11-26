using System.Collections.Generic;
using UnityEngine;

namespace Twoz.Viven.Interactions.Interactor
{
    /// <summary>
    /// 오브젝트를 붙일 수 있는 지점을 나타내는 클래스입니다.
    /// </summary>
    /// <remarks>
    /// <see cref="VivenGrabbableModule"/>을 사용하는 오브젝트는 다른 물체를 붙여 연결할 수 있습니다. AttachPoint를 설정해 다른 오브젝트가 붙을 수 있는 위치를 설정할 수 있습니다.
    /// AttachPoint를 설정하지 않는 경우 물체를 붙일 수 없습니다.
    /// Attach된 오브젝트는 AttachPoint의 부모 GrabbableModule을 따라 움직이며, 물리적 상태가 동기화됩니다.
    ///
    /// <para>
    /// <see cref="attachablePrefabs"/>과 <see cref="notAttachablePrefabs"/>을 사용해 Attach 가능한 오브젝트와 불가능한 오브젝트를 설정할 수 있습니다.
    /// <c>attachablePrefabs</c>와 <c>notAttachablePrefabs</c>는 VObject 빌드시 입력한 오브젝트의 CttId를 사용해 Prefab을 구분합니다.
    /// <c>attachablePrefabs</c>는 화이트리스트, <c>notAttachablePrefabs</c>는 블랙리스트로 작동합니다. 둘 다 설정되어 있는 경우 <c>attachablePrefabs</c> 우선합니다. (<c>notAttachablePrefabs</c>는 무시됩니다.)
    /// <example>
    /// 활 오브젝트에는 화살 오브젝트 만을 붙이고 싶은 경우 attachablePrefabs에 화살 오브젝트의 CttId를 입력합니다.
    /// </example>
    /// </para>
    /// </remarks>
    [AddComponentMenu("VivenSDK/Interaction/Viven Attach Point")]
    [RequireComponent(typeof(Collider))]
    public class VivenAttachPoint : MonoBehaviour
    {
        /// <summary>
        /// attach 가능한 prefab의 object id list
        /// </summary>
        [Tooltip("attach 가능한 prefab list")]
        [SerializeField] public List<string> attachablePrefabs;
        
        /// <summary>
        /// attach 불가능한 prefab의 object id list
        /// </summary>
        [Tooltip("attach 불가능한 prefab list")]
        [SerializeField] public List<string> notAttachablePrefabs;
    }
}