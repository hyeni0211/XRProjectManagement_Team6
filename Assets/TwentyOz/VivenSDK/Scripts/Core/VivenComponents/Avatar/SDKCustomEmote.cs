using System;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [Serializable]
    public class SDKCustomEmote
    {
        [SerializeField] private string emoteName;
        [SerializeField] private Sprite emoteSprite;
        [SerializeField] private AnimationClip emoteClip;
    }
}