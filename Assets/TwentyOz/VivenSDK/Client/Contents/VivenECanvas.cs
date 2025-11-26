using UnityEngine;

namespace Twoz.Viven.ECanvas
{
    /// <summary>
    /// 전자칠판 사용방법
    /// <br/>Assets/TwentyOz/Sample/ElectronicBlackboard/VivenECanvas Prefab을 Unity Scene에 배치
    /// <br/>- 교수자의 경우, 전자칠판은 자동으로 웹서버와 연결됩니다. 네트워크 연결이 원활하지 않은 경우 웹서버와 연결이 되지 않을 수 있습니다.
    /// <br/>- 학생의 경우, 교수자가 스트리밍을 시작하면 전자칠판이 활성화됩니다.
    /// <br/>- 전자칠판이 켜진 후 YouTube 스트리밍 주소 설정 시 학생들의 전자칠판에 스트리밍 영상이 전달됩니다.
    /// <br/>    - 교수자를 제외한 학생들은 전자칠판을 조작할 수 없습니다. 
    /// <br/>
    /// 
    /// <para>
    /// <c>SetRole(string roleName)</c> 함수를 사용하여 교수자와 학생의 역할을 설정할 수 있습니다.
    /// <br/>Presenter : 교수자
    /// <br/>Viewer : 학생
    /// </para>
    /// <br/>
    ///
    /// <see cref="TwentyOz.VivenSDK.Scripts.Core.Contents.ElectronicBlackboard"/>는 이전 버전의 전자칠판입니다. 
    /// </summary>
    public class VivenECanvas : MonoBehaviour
    {
        /// <summary>
        /// 전자칠판이 초기화되었는지 여부를 나타냅니다.
        /// SetRole과 ApplyStreamingSetting을 호출하기 전에 전자칠판이 초기화되었는지 확인할 수 있습니다.
        /// </summary>
        public bool IsECanvasInitialized;

        /// <summary>
        /// 전자칠판의 UI를 간소화하여 사용할 수 있습니다.
        /// </summary>
        public bool isSimpleMode = true;

        /// <summary>
        /// 강의자 전자칠판의 초기 URL입니다.
        /// </summary>
        public string eCanvasStartUrl;

        /// <summary>
        /// <para>
        /// <c>SetRole(string roleName)</c> 함수를 사용하여 교수자와 학생의 역할을 설정할 수 있습니다.
        /// <br/>Presenter : 교수자
        /// <br/>Viewer : 학생
        /// </para>
        /// </summary>
        /// <param name="roleName"></param>
        public void SetRole(string roleName)
        {
        }

        /// <summary>
        /// 스트리밍 키와 YouTube 스트리밍 주소를 설정합니다.
        /// </summary>
        /// <param name="streamingKey"> YouTube 스트리밍 키 </param>
        /// <param name="streamingUrl"> YouTube 스트리밍 주소 (학생들이 전자칠판에 보게 될 스트리밍 주소) </param>
        public void ApplyStreamingSetting(string streamingKey, string streamingUrl)
        {
        }

        /// <summary>
        /// 스트리밍을 시작합니다.
        /// 역할이 교수자 일때만 스트리밍을 시작할 수 있습니다.
        /// </summary>
        public void StartStreaming()
        {
        }

        /// <summary>
        /// 스트리밍을 중지합니다.
        /// 역할이 교수자 일때만 스트리밍을 시작할 수 있습니다.
        /// </summary>
        public void StopStreaming()
        {
        }


        /// <summary>
        /// 전자칠판의 페이지를 설정합니다.
        /// 역할이 교수자 일때만 페이지를 설정할 수 있습니다.
        /// </summary>
        public void SetECanvasPage(string url)
        {
        }

        /// <summary>
        /// 전자칠판의 오디오 입력을 설정합니다.
        /// 역할이 교수자 일때만 오디오 입력을 설정할 수 있습니다.
        /// </summary>
        /// <param name="use">false이면 무음으로 송출됩니다.</param>
        public void UseECanvasAudioInput(bool use)
        {
        }
    }
}