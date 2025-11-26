using System;
using System.Threading.Tasks;
using Twoz.Viven.Interactions;
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
    /// <list type="bullet">
    /// <item>
    ///     <term>SetUrlForNetwork(string)</term>
    ///     <description>네트워크를 통해 방에 있는 유저의 url을 설정합니다.</description>
    /// </item>
    /// <item>
    ///     <term>SetUrlOthers(string)</term>
    ///     <description>네트워크를 통해 나를 제외한 방에 있는 유저의 url을 설정합니다.</description>
    /// </item>
    /// </list>
    /// 예제는 다음과 같습니다.<br/>
    /// <br/>
    /// <code language="lua">
    /// --네트워크를 통해 다른 유저의 웹뷰를 제어하는 예제입니다.
    ///
    /// local webView
    /// function start()
    /// webView = self:GetComponentInChildren(typeof(VivenWebView))
    /// -- 네트워크를 통해 나를 제외한 방에 있는 유저의 url을 설정합니다.
    /// webView:SetUrlOthers("http://google.com")
    /// -- 버튼을 클릭하면 네트워크를 통해 방에 있는 모든 유저의 url을 설정합니다.
    /// self:GetComponentInChildren(typeof(Button)).onClick:AddListener(function()
    /// -- 네트워크를 통해 방에 있는 유저의 url을 설정합니다.
    /// webView:SetUrlForNetwork("http://youtube.com")
    /// end)
    /// end
    /// </code>
    /// </remarks>
    [RequireComponent(typeof(VObject))]
    [AddComponentMenu("VivenSDK/Utility/Viven Web View")]
    public class VivenWebView : MonoBehaviour
    {
        /// <value>
        /// WebView의 초기 URL입니다.
        /// </value>
        public string url;


        /// <summary>
        /// 해당 웹뷰의 url을 설정합니다.
        /// 방에 있는 모든 유저의 url이 설정됩니다.
        /// </summary>
        /// <param name="url">설정할 url</param>
        public void SetUrl(string url)
        {
            
        }

        /// <summary>
        /// 네트워크를 통해 방에 있는 유저의 url을 설정합니다.
        /// </summary>
        /// <param name="url">설정할 url</param>
        [Obsolete("이 클래스는 더 이상 사용되지 않습니다. VivenWebView.SetUrl를 사용하세요.")]
        public void SetUrlForNetwork(string url)
        {
        }

        /// <summary>
        /// 네트워크를 통해 나를 제외한 방에 있는 유저의 url을 설정합니다.
        /// </summary>
        /// <param name="url">설정할 url</param>
        [Obsolete("이 클래스는 더 이상 사용되지 않습니다. VivenWebView.SetUrl를 사용하세요.")]
        public void SetUrlOthers(string url)
        {
            
        }

        /// <summary>
        /// WebView 화면을 캡쳐해서 base64 인코딩합니다.
        /// </summary>
        /// <param name="onCompleted">base64로 인코딩된 이미지를 전달받을 함수</param>
        public async Task CaptureScreenReturnBase64(Action<string> onCompleted = null)
        {
        }
    }
}
