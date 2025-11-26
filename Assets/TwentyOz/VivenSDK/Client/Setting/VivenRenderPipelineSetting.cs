using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Twoz.Viven.Common.Setting
{
    /// <summary>
    /// VMap 에서 사용할 Graphics Setting을 설정합니다. QualitySetting에 따라 RenderPipelineAsset을 사용할 수 있습니다.
    /// </summary>
    [Serializable]
    public struct VivenRenderPipelineSetting
    {
        /// <summary>
        /// PC High 퀄리티
        /// </summary>
        [SerializeField] public RenderPipelineAsset PC_High;
        /// <summary>
        /// PC Medium 퀄리티
        /// </summary>
        [SerializeField] public RenderPipelineAsset PC_Medium;
        /// <summary>
        /// PC Low 퀄리티
        /// </summary>
        [SerializeField] public RenderPipelineAsset PC_Low;
        /// <summary>
        /// Mobile High 퀄리티
        /// </summary>
        [SerializeField] public RenderPipelineAsset Mobile_High;
        /// <summary>
        /// Mobile Medium 퀄리티
        /// </summary>
        [SerializeField] public RenderPipelineAsset Mobile_Medium;
        /// <summary>
        /// Mobile Low 퀄리티
        /// </summary>
        [SerializeField] public RenderPipelineAsset Mobile_Low;
       
        /// <summary>
        /// Viven 에서 사용할 퀄리티 단계입니다. 변수 선언 순서와 QualityLevel을 일치시켜야 합니다.
        /// </summary>
        public static class RenderPipelineLevel
        {
            public const int PCLow = 0;
            public const int PCMedium = 1;
            public const int PCHigh = 2;
            public const int MobileLow = 3;
            public const int MobileMedium = 4;
            public const int MobileHigh = 5;
        }

        public RenderPipelineAsset GetRenderPipelineAsset(int qualityLevel)
        {
            return qualityLevel switch
            {
                0 => PC_Low,
                1 => PC_Medium,
                2 => PC_High,
                3 => Mobile_Low,
                4 => Mobile_Medium,
                5 => Mobile_High,
                _ => throw new ArgumentOutOfRangeException(nameof(qualityLevel), qualityLevel, null)
            };
        }
        
        public void AddRenderPipelineAsset(RenderPipelineAsset renderPipelineAsset, int qualityLvel)
        {
            switch (qualityLvel)
            {
                case 0:
                    PC_Low = renderPipelineAsset;
                    break;
                case 1:
                    PC_Medium = renderPipelineAsset;
                    break;
                case 2:
                    PC_High = renderPipelineAsset;
                    break;
                case 3:
                    Mobile_Low = renderPipelineAsset;
                    break;
                case 4:
                    Mobile_Medium = renderPipelineAsset;
                    break;
                case 5:
                    Mobile_High = renderPipelineAsset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(qualityLvel), qualityLvel, null);
            }
        }
        
        /// <summary>
        /// VMap에서 사용할 RenderPipelineSetting을 가져옵니다.
        /// </summary>
        /// <remarks>
        /// ProjectSettings/Quality에서 설정한 RenderPipelineAsset을  RenderPipelineSetting을 생성합니다.
        /// </remarks>
        /// <returns>VMap에서 사용할 Graphics 설정</returns>
        /// <exception cref="Exception"></exception>
        public static VivenRenderPipelineSetting GetRenderPipelineSetting()
        {
            var renderPipelineSetting = new VivenRenderPipelineSetting();

            // QualitySettings에서 RenderPipelineAsset을 가져와 RenderPipelineSetting에 추가합니다.
            var renderPipelineAssetPCHigh =
                QualitySettings.GetRenderPipelineAssetAt(VivenRenderPipelineSetting.RenderPipelineLevel.PCHigh);
            renderPipelineSetting.AddRenderPipelineAsset(renderPipelineAssetPCHigh,
                VivenRenderPipelineSetting.RenderPipelineLevel.PCHigh);

            var renderPipelineAssetPCMedium =
                QualitySettings.GetRenderPipelineAssetAt(VivenRenderPipelineSetting.RenderPipelineLevel.PCMedium);
            renderPipelineSetting.AddRenderPipelineAsset(renderPipelineAssetPCMedium,
                VivenRenderPipelineSetting.RenderPipelineLevel.PCMedium);

            var renderPipelineAssetPCLow =
                QualitySettings.GetRenderPipelineAssetAt(VivenRenderPipelineSetting.RenderPipelineLevel.PCLow);
            renderPipelineSetting.AddRenderPipelineAsset(renderPipelineAssetPCLow,
                VivenRenderPipelineSetting.RenderPipelineLevel.PCLow);

            var renderPipelineAssetMobileHigh =
                QualitySettings.GetRenderPipelineAssetAt(VivenRenderPipelineSetting.RenderPipelineLevel.MobileHigh);
            renderPipelineSetting.AddRenderPipelineAsset(renderPipelineAssetMobileHigh,
                VivenRenderPipelineSetting.RenderPipelineLevel.MobileHigh);

            var renderPipelineAssetMobileMedium =
                QualitySettings.GetRenderPipelineAssetAt(VivenRenderPipelineSetting.RenderPipelineLevel.MobileMedium);
            renderPipelineSetting.AddRenderPipelineAsset(renderPipelineAssetMobileMedium,
                VivenRenderPipelineSetting.RenderPipelineLevel.MobileMedium);

            var renderPipelineAssetMobileLow =
                QualitySettings.GetRenderPipelineAssetAt(VivenRenderPipelineSetting.RenderPipelineLevel.MobileLow);
            renderPipelineSetting.AddRenderPipelineAsset(renderPipelineAssetMobileLow,
                VivenRenderPipelineSetting.RenderPipelineLevel.MobileLow);

            // RenderPipelineAsset이 null이면 에러를 발생시킵니다.
            for (var qualityLevel = 0; qualityLevel < 6; qualityLevel++)
            {
                // RenderPipelineAsset이 null인 지 확인합니다.
                if (renderPipelineSetting.GetRenderPipelineAsset(qualityLevel) != null) continue;
                Debug.LogError($"RenderPipelineAsset:{qualityLevel} is null");
                throw new Exception($"RenderPipelineAsset:{qualityLevel} is null");
            }

            return renderPipelineSetting;
        }
    }
    
}