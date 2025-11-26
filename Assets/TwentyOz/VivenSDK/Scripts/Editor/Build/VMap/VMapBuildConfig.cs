using System;
using System.Collections.Generic;
using TwentyOz.VivenSDK.Scripts.Core.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace TwentyOz.VivenSDK.Scripts.Editor.Build.VMap
{
    [CreateAssetMenu(menuName = "VivenSDK/VMapBuildConfig", fileName = "new VMapBuildConfig", order = 0)]
    [Serializable]
    public class VMapBuildConfig : ScriptableObject
    {
        /// <summary>
        /// BuildSetting의 이름입니다.
        /// 기본 탭은 Default로 설정되어 있습니다.
        /// </summary>
        public string ConfigName;

        /// <summary>
        /// 빌드 조건을 검사합니다.
        /// </summary>
        public Func<bool> Condition;

        /// <summary>
        /// 빌드 시작시 호출됩니다.
        /// </summary>
        public Action Start;

        /// <summary>
        /// 빌드가 끝나고 압축전에 호출됩니다.
        /// </summary>
        public Action End;

        /// <summary>
        /// 추가적인 프로퍼티를 설정합니다.
        /// </summary>
        protected List<VivenContentProperty> AdditionalProperties = new();

        /// <summary>
        /// VMap 빌드 설정 창에서 추가적인 UI를 그리기 위한 델리게이트
        /// </summary>
        public BuildSettingDelegate BuildTabUi = visualElement =>
        {
            visualElement.Add(new Label("기본적인 Viven Map을 빌드하기 위한 설정입니다."));
        };
        
        /// <summary>
        /// 추가적인 Pack에서 GUI를 새로 그리기 위해서 사용할 델리게이트
        /// 추가적인 Tab을 생성하고싶으면 Tabs에 추가하시고,
        /// 추가적인 GUI를 Delegates에 추가해주시면 됩니다.
        ///
        /// 사용예시:
        /// VivenMapBuildSettingWindow.Tabs.Add("KHU"); --> 추가적인 탭을 생성합니다.
        /// </summary>
        public delegate void BuildSettingDelegate(VisualElement additionRoot);
        
        public virtual List<VivenContentProperty> GetContentProperties()
        {
            return new List<VivenContentProperty>();
        }
    }
}