using UnityEngine;
using UnityEngine.Serialization;

namespace Twoz.Viven.Audio.Data
{
    /// <summary>
    /// Viven에서 FMOD의 EventInstance를 사용하기 위한 클래스입니다.
    /// </summary>
    /// <remarks>
    /// 사용할 AudioClip을 설정하고, 오디오 믹서 그룹을 설정할 수 있습니다.
    /// AudioClip은 FMOD의 EventInstance로 변환되어 재생됩니다.
    ///
    /// AudioClip의 LoadType은 DecompressOnLoad로 설정되어야 합니다.
    /// FMOD에서 AudioClip을 동적으로 변환하기 위해서는 LoadType이 DecompressOnLoad로 설정되어야 합니다.
    /// <para>
    /// FMOD의 EventInstance를 확장된 기능을 사용할 수 있습니다.
    /// </para> 
    /// </remarks>
    [AddComponentMenu("VivenSDK/Audio/Viven Audio Event Instance")]
    public class VivenAudioEventInstance : MonoBehaviour
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
        /// 시작 시 자동 재생 설정입니다.
        /// </summary>
        [SerializeField] public bool autoPlayOnStart;

        /// <summary>
        /// 반복 여부 설정입니다.
        /// </summary>
        [SerializeField] public bool isLoopingOnStart;

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
        public void Play(bool looping = false) { }

        /// <summary>
        /// FMOD의 EventInstance를 1회 재생합니다. (Looping 불가)
        /// </summary>
        public void PlayOneShot() { }
        
        /// <summary>
        /// FMOD의 EventInstance를 정지합니다.
        /// </summary>
        public void Stop() { }
    }
} 