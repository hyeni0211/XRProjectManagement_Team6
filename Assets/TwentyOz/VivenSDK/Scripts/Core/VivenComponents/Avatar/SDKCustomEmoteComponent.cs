using System.Collections.Generic;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    [AddComponentMenu("VivenSDK/Avatar/CustomEmoteComponent")]
    public class SDKCustomEmoteComponent : MonoBehaviour
    {
        /// <summary>
        /// 런타임 수정사항은 적용되지 않습니다.
        /// </summary>
        [SerializeField] private List<SDKCustomEmote> sdkCustomEmotes;
    }
}