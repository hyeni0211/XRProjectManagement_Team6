using System;
using UnityEngine;

namespace Twoz.Viven.HandTracking.DataModels
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "TwozHandMotionGesture", menuName = "Viven/Create HandMotionGesture", order = 1)]
    public class VivenHandMotionGesture : ScriptableObject
    {
        public VivenHandPose startHandPose;
        
        public VivenHandPose endHandPose;
        
        /// <summary>
        /// startHandPose가 감지되고 endHandPose가 감지되기까지의 최대 시간
        /// </summary>
        public float maximumTransitionTime = 0.5f;
    }
}