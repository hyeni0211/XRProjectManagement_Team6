using System;
using UnityEngine;

namespace Twoz.Viven.HandTracking.DataModels
{
    [Serializable]
    public class VivenTargetRelativeOrientation
    {
        /// <summary>
        /// pose를 판단하기 위해 사용되는 hand axis
        /// </summary>
        public VivenHandAxis handAxis;

        /// <summary>
        /// hand pose를 판단하기 위해 hand axis와 비교할 reference axis 
        /// </summary>
        public VivenReferenceTargetAxis referenceAxis;

        /// <summary>
        /// hand axis와 reference axis 비교 방법
        /// </summary>
        public VivenHandAlignment alignment;

        /// <summary>
        /// desiredAngle에 대한 허용 오차
        /// </summary>
        [Range(0.1f, 180f)]
        public float angleTolerance;

        /// <summary>
        /// 무시하려고 하는 특정 축. 예를 들어, y축을 무시하면 x-z 평면에서 판단함.
        /// </summary>
        public VivenIgnoreAxis ignoreAxis;
    }
}