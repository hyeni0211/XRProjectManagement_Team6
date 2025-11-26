using Twoz.Viven.Common.Setting;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Common
{
    public class VivenMapEnvironment : MonoBehaviour
    {
        /// <summary>
        /// 해당 옵션이 활성화 된다면 Viven의 기본적인 Skybox를 사용합니다. 그렇지 않다면 사용자가 직접 Skybox를 설정해야 합니다.
        /// </summary>
        public bool useSky = true;

        /// <summary>
        /// 해당 옵션이 활성화 된다면 Viven의 기본적인 PostProcess를 사용합니다. 그렇지 않다면 사용자가 직접 PostProcess를 설정해야 합니다.
        /// </summary>
        public bool useVivenPostProcess = true;

        /// <summary>
        /// map의 property를 설정합니다.
        /// 해당 값들은 WebPage에서 맵 생성시 설정할 수 있습니다.
        /// <br/><br/><b>TODO</b>: 추후 Dictionary형태로 변경할 예정입니다.(중복키 방지), 추가적으로 현재는 string만 지원하지만, 추후에는 다른 타입도 지원할 예정입니다.
        /// </summary>
        public VivenContentProperty[] mapProperties;

        /// <summary>
        /// QualitySetting에 따라 사용할 RenderPipelineSetting입니다.
        /// </summary>
        [HideInInspector] public VivenRenderPipelineSetting renderPipelineSetting;
    }
}