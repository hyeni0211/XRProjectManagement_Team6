using System.Collections.Generic;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [AddComponentMenu("VivenSDK/Avatar/OutfitComponent")]
    public class SDKOutfitComponent : MonoBehaviour 
    {
        [SerializeField] private List<SDKOutfit> sdkOutfits;
    }
}