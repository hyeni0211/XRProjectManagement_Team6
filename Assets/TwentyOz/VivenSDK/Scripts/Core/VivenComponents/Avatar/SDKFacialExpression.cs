using System;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [Serializable]
    public class SDKFacialExpression
    {
        [SerializeField] private string expressionName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private float[] blendShapeValues;

        public float[] BlendShapeValues
        {
            get => blendShapeValues;
            set => blendShapeValues = value;
        }

        public string ExpressionName
        {
            get => expressionName;
            set => expressionName = value;
        }
    }
}