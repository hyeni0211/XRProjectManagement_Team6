using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Twoz.Viven.Audio.Data
{
    [Obsolete("VivenAudioEventInstance를 사용하세요.")]
    public class VivenEventInstance : MonoBehaviour
    {
        /// <summary>
        /// AudioClip이 재생될 오디오 믹서 그룹. MixerGroupType에 따라 음량을 조절할 수 있습니다.
        /// </summary>
        [FormerlySerializedAs("mixerGroupType")] 
        [SerializeField] public VivenAudioMixerGroupType groupType;
        
        /// <summary>
        /// 재생할 AudioClip.
        /// </summary>
        [SerializeField] public AudioClip audioClip;

        /// <summary>
        /// FMOD의 EventInstance를 재생합니다.
        /// 그룹별로 오디오 최대길이는 다음과 같습니다.
        /// <list type="bullet">
        /// <item>
        /// <term>Default</term>
        /// <description>2분 30초</description>
        /// </item>
        /// <item>
        /// <term>Environment</term>
        /// <description>5초</description>
        /// </item>
        /// <item>
        /// <term>Sfx</term>
        /// <description>5초</description>
        /// </item>
        /// <item>
        /// <term>Bgm</term>
        /// <description>5초</description>
        /// </item>
        /// </list>
        /// </summary>
        public void Play() { }

        /// <summary>
        /// FMOD의 EventInstance를 1회 재생합니다. (Looping 불가)
        /// </summary>
        public void PlayOneShot() { }
        
        [Obsolete("Play(looping) 을 사용하세요")]
        public void PlayBGM(bool looping = true) { }
        
        /// <summary>
        /// FMOD의 EventInstance를 정지합니다.
        /// </summary>
        public void Stop() { }
    }
}