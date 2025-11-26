using System;
using UnityEngine;

namespace Twoz.Viven.HandTracking.DataModels
{
    [Serializable, CreateAssetMenu(fileName = "TwozHandPose", menuName = "Viven/Create HandPose", order = 1)]
    public class VivenHandPose : ScriptableObject
    {

        /// <summary>
        /// 감지하려는 hand pose의 손 모양 
        /// </summary>
        [SerializeField] private ScriptableObject handShapeObj;

        /// <summary>
        /// 감지하려는 target object를 기준으로 한 hand rotation
        /// </summary>
        [SerializeField] public VivenTargetRelativeOrientation[] desiredTargetOrientation;
        
        /// <summary>
        /// 감지하려는 유저를 기준으로 한 hand rotation 
        /// </summary>
        [SerializeField]
        public VivenUserRelativeOrientation[] desiredUserOrientation;

        /// <summary>
        /// 손 모양이 감지되었을 때 최소로 유지해야 하는 시간
        /// </summary>
        public float minimumHoldTime = 0.2f;
    }
}