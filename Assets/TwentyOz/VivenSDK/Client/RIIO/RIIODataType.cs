namespace Twoz.Viven.RuntimeInteractionObject.Core
{
    /// <summary>
    /// RIIO Receiver와 Sender에서 전달할 수 있는 데이터 타입을 나타냅니다.
    /// </summary>
    public enum RIIODataType
    {
        /// <summary> 데이터를 전달하지 않음(이벤트만 호출) </summary>
        Void,
        /// <summary> 실수형 </summary>
        Number,
        /// <summary> 문자열 </summary>
        String,
        /// <summary> bool값 </summary>
        Bool
    }
}