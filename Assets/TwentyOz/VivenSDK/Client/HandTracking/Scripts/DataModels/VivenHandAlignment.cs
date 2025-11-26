namespace Twoz.Viven.HandTracking.DataModels
{
    /// <summary>
    /// 두 손의 방향을 나타내는 enum입니다.
    /// </summary>
    public enum VivenHandAlignment
    {
        /// <summary> 같은 방향 </summary>
        AlignsWith,
        /// <summary> 반드시 서로 수직 </summary>
        PerpendicularTo,
        /// <summary> 평행하지만 반대 방향 </summary>
        OppositeTo,
    }
}