using UnityEngine;
using TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Interactions
{
    /// <summary>
    /// VObject의 Outline을 제어하는 모듈입니다.
    /// </summary>
    /// <remarks>
    /// VObject는 상호작용 상태를 표시하기 위해 Outline을 사용합니다.
    /// 현재 상호작용 중인 interactor에 따라 Outline의 색상이 변경됩니다.
    /// Outline은 interactor에 의해 제어되며, OutlineModule은 Outline의 너비와 색상을 설정합니다.
    ///
    /// <para>
    /// VivenBehaviour에서 OutlineModule을 제어할 수 있지만, Grabbable 오브젝트에서 OutlineModule을 제어하는 것은 권장되지 않습니다.
    /// </para>
    /// </remarks>
    [AddComponentMenu("VivenSDK/Utility/Outline Module")]
    public class OutlineModule : MonoBehaviour
    {
        /// <summary>
        /// Outline의 굵기입니다.
        /// </summary>
        [Range(0, 10)] [SerializeField] private float outlineWidth = 2f;

        /// <summary>
        /// Outline의 색상입니다.
        /// </summary>
        [SerializeField] private Color outlineColor = Color.blue;
        
        /// <summary>
        /// Outline의 모드입니다.
        /// </summary>
        [SerializeField] private OutlineModeType outlineMode = OutlineModeType.OutlineAll;

    }
}