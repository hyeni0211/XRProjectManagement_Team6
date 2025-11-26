using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.UI
{
    /// <summary>
    /// XR, PC 모드일 때에 모드에 맞게 UI를 변경합니다.
    /// </summary>
    /// <remarks>
    /// XR 모드일 때는 XR UI를 활성화하고 PC 모드일 때는 PC UI를 활성화합니다.
    /// </remarks>
    [AddComponentMenu("VivenSDK/UI/UI Mode Changer")]
    public class UIModeChanger : MonoBehaviour
    {
        [SerializeField] public GameObject xrUI;
        [SerializeField] public GameObject pcUI;
    }
}
