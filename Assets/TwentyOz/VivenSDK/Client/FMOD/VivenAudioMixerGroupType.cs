namespace Twoz.Viven.Audio
{
    /// <summary>
    /// Viven 에서 사용하는 효과음 종류입니다.
    /// <para>오디오 설정에서 각 카테고리의 음량을 조정할 수 있습니다.</para>
    /// 기본(Default) : 효과음으로 설정됨
    /// </summary>
    public enum VivenAudioMixerGroupType
    {
        /// <summary>
        /// 기본 (효과음으로 설정됨)
        /// </summary>
        Default,
        /// <summary>
        /// 환경음
        /// </summary>
        Environment,
        /// <summary>
        /// 효과음
        /// </summary>
        Sfx,
        /// <summary>
        /// 배경음
        /// </summary>
        Bgm
    }
}