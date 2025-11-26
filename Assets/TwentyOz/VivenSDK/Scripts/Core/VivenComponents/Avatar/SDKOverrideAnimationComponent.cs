using System.Collections.Generic;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [AddComponentMenu("VivenSDK/Avatar/OverrideAnimationComponent")]
    public class SDKOverrideAnimationComponent : MonoBehaviour
    {
        [SerializeField] private SDKOverrideAnimation sdkOverrideAnimation;

        public SDKOverrideAnimation OverrideAnimation => sdkOverrideAnimation;
    }
}