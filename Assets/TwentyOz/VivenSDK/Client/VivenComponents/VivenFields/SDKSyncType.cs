namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields
{
    public enum SDKSyncType
    {
        // TODO! Manual 동기화 함수 추가
        /// <summary>
        /// 매 틱(Tick)마다 동기화합니다. Continuous 동기화가 많아지면 네트워크가 느려질 수 있습니다.
        /// </summary>
        Continuous,
        /// <summary>
        /// 수동으로 동기화합니다. 동기화 함수 " "를 호출해 수동으로 데이터를 동기화합니다.
        /// </summary>
        Manual,
        /// <summary>
        /// 값이 변경될 때 동기화합니다. 값이 변경될 때만 동기화하므로, 네트워크 부하가 적습니다.
        /// </summary>
        OnChanged
    }
}