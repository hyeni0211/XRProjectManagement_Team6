namespace Twoz.Viven.HandTracking.DataModels
{
    /// <summary>
    /// 감지된 손이 왼손인 지, 오른손인 지, 양손인 지를 나타내는 enum입니다.
    /// </summary>
    public enum VivenHandedness
    {
        /// <summary> 감지되지 않음 </summary>
        None,
        /// <summary> 오른손 </summary>
        Right,
        /// <summary> 왼손 </summary>
        Left,
        /// <summary> 양손 </summary>
        Both
    }
}