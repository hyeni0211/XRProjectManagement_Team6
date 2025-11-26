using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [Serializable]
    public class SDKOutfit
    {
        [SerializeField] private bool isDefault;
        [SerializeField] private string presetName;
        [SerializeField] private List<SDKClothData> clothData;
        [SerializeField] private Sprite thumbnail;
    }
}