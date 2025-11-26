using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.WebView
{
    /// <summary>
    /// Viven에서 WebView를 사용하기 위한 컴포넌트입니다.
    /// </summary>
    /// <remarks>
    /// SDK에서 WebView를 사용하기 위한 컴포넌트입니다.
    /// Ui Canvas에 컴포넌트를 추가하면 해당 URL로 WebView를 띄울 수 있습니다.
    ///
    /// lua에서 사용할 수 있는 API는 다음과 같습니다.
    /// 예제는 다음과 같습니다.<br/>
    /// <br/>
    /// <code language="lua">
    /// local webView
    /// function start()
    /// webView = self:GetComponentInChildren(typeof(VivenLocalWebView))
    /// -- 내 WebView의 Url을 설정합니다.
    /// webView:SetUrl("http://google.com")
    /// end
    /// </code>
    /// </remarks>
    [AddComponentMenu("VivenSDK/Utility/Viven Local Web View")]
    public class VivenLocalWebView : MonoBehaviour
    {
        /// <value>
        /// WebView의 초기 URL입니다.
        /// </value>
        public string url;
        
        /// <summary>
        /// 네트워크를 통해 방에 있는 유저의 url을 설정합니다.
        /// </summary>
        /// <param name="url">설정할 url</param>
        public void SetUrl(string url)
        {
        }
    }
}
