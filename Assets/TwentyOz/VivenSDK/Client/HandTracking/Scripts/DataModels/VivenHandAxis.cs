namespace Twoz.Viven.HandTracking.DataModels
{
    /// <summary>
    /// 손의 축을 나타내는 enum입니다.
    /// </summary>
    public enum VivenHandAxis
    {
        /// <summary> palm의 forward </summary>
        PalmDirection,
        /// <summary> 완전히 쭉 핀 엄지 손가락 방향. 왼손/오른손 방향이 다름 </summary> 
        ThumbExtendedDirection,
        /// <summary> 완전히 쭉 핀 손의 proximal -> distal </summary> 
        FingersExtendedDirection,
    }
}