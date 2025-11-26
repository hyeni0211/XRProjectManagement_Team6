namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.VivenFields
{
    /// <summary>
    /// TwozVObject의 Content Type을 정의합니다.
    /// </summary>
    /// <remarks>
    /// <para>
    /// VObject는 Viven에 업로드되어 다운로드해 사용할 수 있는 오브젝트입니다.
    /// 맵에 상관없이 사용할 오브젝트는 VObject로 빌드해 업로드해야 합니다.
    /// VObject라는 명칭은 TwozVObject와는 별개의 용어입니다. TwozVObject는 네트워크에 동기화되어 상호작용할 수 있는 모든 오브젝트를 의미합니다.
    /// </para>
    /// <para>
    /// Prepared는 맵에 배치되어 함께 업로드되는 오브젝트입니다.
    /// Prepared 오브젝트는 오브젝트가 빌드된 맵에서만 사용할 수 있고, 다른 맵에서는 불러올 수 없습니다. 
    /// </para>
    /// </remarks>
    public enum SDKContentType
    {
        /// <value>
        /// 맵과 함께 빌드되는 오브젝트
        /// </value>
        Prepared,
        /// <value>
        /// 오브젝트 메뉴에 업로드되는 오브젝트
        /// </value>
        VObject,
        /// <summary>
        /// VMap 빌드 시 포함되는 오브젝트입니다.
        /// </summary>
        MapContent,
    }
}