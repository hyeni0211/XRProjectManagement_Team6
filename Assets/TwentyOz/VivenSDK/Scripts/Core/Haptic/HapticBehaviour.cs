using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Haptic
{
    public class HapticBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 얼마나 부드러운 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.
        /// </summary>
        [Tooltip("얼마나 부드러운 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.")]
        public float smoothness;

        /// <summary>
        /// 얼마나 따뜻한 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.
        /// </summary>
        [Tooltip("얼마나 따뜻한 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.")]
        public float warmness;

        /// <summary>
        /// 얼마나 딱딱한 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.
        /// </summary>
        [Tooltip("얼마나 딱딱한 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.")]
        public float hardness;
        
        /// <summary>
        /// 얼마나 미끄러운 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.
        /// </summary>
        [Tooltip("얼마나 미끄러운 물체인지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.")]
        public float friction;
        
        /// <summary>
        /// 얼마나 강한 진동을 내는지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.
        /// </summary>
        [Tooltip("얼마나 강한 진동을 내는지 결정합니다. 0.0f ~ 1.0f 사이의 값을 가집니다.")]
        public float hapticIntensity = 1.0f;

        [Tooltip("만약 true라면, Awake()에서 자동으로 HapticManager에 등록됩니다.")]
        public bool autoPopulateOnAwake = true;
    }
}