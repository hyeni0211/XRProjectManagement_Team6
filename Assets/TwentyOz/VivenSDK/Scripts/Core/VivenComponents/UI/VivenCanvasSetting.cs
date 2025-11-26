using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.UI
{
    /// <summary>
    /// Canvas를 설정하는 컴포넌트
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [AddComponentMenu("VivenSDK/UI/Viven Canvas Setting")]
    public class VivenCanvasSetting : MonoBehaviour
    {
        /// <summary>
        /// 항상 앞에 UI의 화면이 보이도록 하는 옵션입니다.
        /// </summary>
        [SerializeField] public bool alwaysFront = true;
    }
}