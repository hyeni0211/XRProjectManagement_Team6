using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.UI
{
    /// <summary>
    /// Viven에서 사용하는 PC, VR용 GraphicRaycaster입니다. 
    /// </summary>
    [AddComponentMenu("VivenSDK/UI/Viven Graphic Raycaster")]
    public class VivenGraphicRaycaster : MonoBehaviour
    {
        /// <summary>
        /// 항상 앞에 UI의 화면이 보이도록 하는 옵션입니다.
        /// </summary>
        [SerializeField] public bool alwaysFront = true;
    }
}