using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Twoz.Viven.Player.PlayerRig.Animation
{
    /// <summary>
    /// VivenBehaviour 에서 내 Avatar의 원하는 애니메이션을 재생할 수 있습니다.
    /// </summary>
    /// <remarks>
    /// PC 모드일 경우 3인칭 모드로 전환되며, VR모드일 경우 1인칭 모드로만 사용됩니다.
    /// <para>Humanoid Avatar의 애니메이션 클립만 사용 가능합니다.</para>
    /// <para>
    /// Custom 애니메이션은 다음과 상태일 경우 의도치 않게 동작할 수 있습니다.
    /// <list type="bullet">
    ///     <item>VMC모드</item>
    ///     <item>RIIO모드</item>
    ///     <item>Emote를 실행 중일 때</item>
    /// </list>
    /// </para>
    /// </remarks>
    [AddComponentMenu("VivenSDK/Animation/Viven Custom Animation Module")]
    public class VivenCustomAnimationModule : MonoBehaviour
    {
        /// <summary>
        /// 컴포넌트에서 사용할 애니메이션 클립 목록
        /// <para>각 클립의 이름은 고유해야 합니다.</para>
        /// </summary>
        [FormerlySerializedAs("SDK_AnimationClips")]
        [Header("애니메이션 목록")]
        [SerializeField]
        public List<VivenPlayableClip> clipList = new();

        /// <summary>
        /// 애니메이션 클립을 재생합니다.
        /// 애니메이션 클립이 Loop 모드일 경우, <see cref="StopAnimation"/>을 호출할 때 까지 반복 재생됩니다.
        /// </summary>
        /// <param name="targetClipName">재생할 애니메이션 클립의 이름</param>
        public void PlayAnimation(String targetClipName)
        {
        }

        /// <summary>
        /// 해당 애니메이션이 재생 중이라면 중지합니다.
        /// </summary>
        /// <param name="targetClipName">중지할 애니메이션 클립의 이름</param>
        public void StopAnimation(String targetClipName)
        {
        }
        
        /// <summary>
        /// PC 모드인 경우 3인칭 애니메이션을 재생합니다.
        /// VR 모드인 경우 1인칭으로 재생합니다.
        /// </summary>
        /// <param name="targetClipName">재생할 애니메이션 클립의 이름</param>
        public void PlayAnimationThirdPerson(String targetClipName)
        {
        }
    }
    
    [Serializable]
    public struct VivenPlayableClip
    {
        /// <summary>
        /// SDK에서 클립 재생에 사용할 이름
        /// </summary>
        public string clipName;
        
        /// <summary>
        /// 재생할 애니메이션 클립
        /// </summary>
        public AnimationClip clip;
    }
}