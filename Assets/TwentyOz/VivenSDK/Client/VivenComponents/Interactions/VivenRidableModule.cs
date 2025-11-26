using System;
using Twoz.Viven.Player.PlayerRig.Animation;
using UnityEngine;
using UnityEngine.Events;

namespace Twoz.Viven.Interactions
{
    /// <exclude /> WIP - SDK 문서에서 제외
    /// <summary>
    /// 탈것에 사용되는 모듈입니다.
    /// </summary>
    [RequireComponent(typeof(VivenCustomAnimationModule))]
    public class VivenRidableModule : MonoBehaviour
    {
        [NonSerialized] public UnityEvent startEvent;
        [NonSerialized] public UnityEvent endEvent;

        [Obsolete][HideInInspector]
        public AnimationClip animation;
    }
}   