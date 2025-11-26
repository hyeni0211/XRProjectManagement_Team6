using Twoz.Viven.HandTracking.DataModels;
using UnityEngine;
using UnityEngine.Events;

namespace Twoz.Viven.HandTracking.Gesture
{
    [AddComponentMenu("VivenSDK/HandTracking/Viven Pose Or Gesture Interaction")]
    public class VivenPoseOrGestureInteraction : MonoBehaviour
    {
        [SerializeField] 
        private ScriptableObject twozHandPoseOrGesture;

        [SerializeField]
        private Transform targetTransform;

        [SerializeField] 
        private VivenHandedness detectHandType;
        
    #region Callback Events

        /// <summary>
        /// 해당 제스처가 감지되었을때
        /// </summary>
        public UnityEvent onPoseOrGesturePerformed;

        /// <summary>
        /// 해당 제스처가 끝났을때
        /// </summary>
        public UnityEvent onPoseOrGestureEnded;

    #endregion
    }
}
