using UnityEngine;
using UnityEngine.Events;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.UI
{
    /// <summary>
    /// UI에 대한 포인터 이벤트를 처리하는 클래스입니다.
    /// 마우스가 아닌 포인터에도 작동하기 위하여 작성하였습니다. (ex. onMouseEnter)
    /// </summary>
    [AddComponentMenu("VivenSDK/UI/VivenUIPointerEvents")]
    public class VivenUIPointerEvents : MonoBehaviour
    {
        /// <summary>
        /// 포인터가 UI에 들어왔을 때 발생하는 이벤트입니다.
        /// </summary>
        [Tooltip("포인터가 UI에 들어왔을 때 발생하는 이벤트입니다.")]
        public UnityEvent onPointerEnter;
        
        /// <summary>
        /// 포인터가 UI에서 나갔을 때 발생하는 이벤트입니다.
        /// </summary>
        [Tooltip("포인터가 UI에서 나갔을 때 발생하는 이벤트입니다.")]
        public UnityEvent onPointerExit;
        
        /// <summary>
        /// 포인터가 UI를 클릭했을 때 발생하는 이벤트입니다.
        /// </summary>
        [Tooltip("포인터가 UI를 클릭했을 때 발생하는 이벤트입니다.")]
        public UnityEvent onPointerClick;
    }
}