namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields
{
    /// <summary>
    /// RPC를 보낼 대상을 정의합니다.
    /// </summary>
    public enum SDKRPCSendOption
    {
        /// <summary>
        /// 같은 방에 있는 모든 플레이어에게 RPC를 보냅니다.(본인 포함)
        /// </summary>
        All,
        /// <summary>
        /// 나를 제외한 모든 플레이어에게 RPC를 보냅니다.
        /// </summary>
        Others,
        /// <summary>
        /// 특정 플레이어에게 RPC를 보냅니다.
        /// 자기 자신에게 RPC를 보낼 경우 Target을 자신의 Id로 설정합니다.
        /// </summary>
        Target
    }
}