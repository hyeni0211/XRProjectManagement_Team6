using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Mirror
{
    /// <summary>
    /// 미러 렌더링 최적화 모드
    /// </summary>
    public enum OptimizationMode
    {
        /// <summary>플랫폼 감지 기반 자동 최적화</summary>
        Auto,
        /// <summary>사용자 직접 설정 (자동 최적화 비활성화)</summary>
        Custom
    }

    /// <summary>
/// VIVEN 환경에서 Mirrors and Reflections for VR의 MirrorRenderer를 래핑하는 컴포넌트입니다.
///
/// - 목적: 외부 패키지 MirrorRenderer를 안전하게 감싸고 VIVEN 규칙(로깅/예외/최적화)을 적용합니다.
/// - 기능: 자동/수동 최적화, 플랫폼 감지, 설정 반영, 성능 친화 기본값 제공.
///
/// 사용 예(C#):
/// <example>
/// <code>
/// var mr = gameObject.GetComponent<SDKMirrorRendererComponent>();
/// mr.Recursions = 2;
/// mr.ScreenScaleFactor = 0.5f;
/// mr.AntiAliasingLevel = 2; // 1=None, 2=Low, 4=Medium, 8=High
/// mr.RenderingEnabled = true;
/// </code>
/// </example>
///
/// 사용 예(Lua, xLua):
/// <example>
/// <code>
/// local mr = gameObject:GetComponent("SDKMirrorRendererComponent")
/// if mr ~= nil then
///   mr.Recursions = 2
///   mr.ScreenScaleFactor = 0.5
///   mr.AntiAliasingLevel = 2
///   mr.RenderingEnabled = true
/// end
/// </code>
/// </example>
/// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("VivenSDK/Mirror/SDKMirrorRendererComponent")]
    public class SDKMirrorRendererComponent : MonoBehaviour
    {
        [Header("Surface Connections")]
        [Tooltip("이 렌더러가 렌더링할 미러 표면들의 목록입니다. 비워두면 씬의 모든 MirrorSurface를 자동으로 감지합니다.")]
        [SerializeField] private List<SDKMirrorSurfaceComponent> _mirrorSurfaces = new();

        [Header("Optimization Settings")]
        [Tooltip("최적화 모드: Auto (자동 플랫폼 감지로 최적 설정 적용), Custom (모든 설정을 수동으로 조정)")]
        [SerializeField] public OptimizationMode optimizationMode = OptimizationMode.Auto;

        [Header("Mirror Quality")]
        [Tooltip("미러 상호 반사 깊이 (1~8). 높을수록 무한 반사 효과가 깊어지지만 성능이 크게 저하됩니다. 모바일: 1-2, VR: 1-3, PC: 2-4 권장")]
        [SerializeField] private int _recursions = 2;

        [Tooltip("미러 해상도 스케일 (0.1~1.0). 낮을수록 성능 향상, 높을수록 선명도 증가. 모바일: 0.25-0.5, VR: 0.5, PC: 0.5-1.0 권장")]
        [Range(0.1f, 1.0f)]
        [SerializeField] private float _screenScaleFactor = 0.5f;

        [Tooltip("안티앨리어싱 레벨: 1=없음(빠름), 2=낮음(권장), 4=중간(PC용), 8=높음(고사양PC용). 모바일에서는 1, VR에서는 1-2 권장")]
        [SerializeField] private int _antiAliasingLevel = 2;

        [Tooltip("미러 렌더링 활성화/비활성화. 비활성화 시 모든 미러가 검은색으로 표시됩니다.")]
        [SerializeField] private bool _renderingEnabled = true;

        [Tooltip("프레임 스킵 횟수 (0=매 프레임 업데이트). 값이 클수록 성능 향상되지만 미러 반응성이 떨어집니다. VR: 0-1, 모바일: 1-3 권장")]
        [Min(0)]
        [SerializeField] private int _frameSkip = 0;

        [Header("Rendering Settings")]
        [Tooltip("미러에 반사될 레이어들을 선택합니다. 불필요한 레이어 제외 시 성능 향상됩니다. (예: UI, 이펙트 레이어 제외)")]
        [SerializeField] private LayerMask _renderLayers = -1;

        [Tooltip("미러에서 그림자를 렌더링할지 설정합니다. 비활성화 시 성능 향상되지만 현실감이 떨어집니다.")]
        [SerializeField] private bool _renderShadows = false;

        [Tooltip("미러에서 포스트 프로세싱 효과를 적용할지 설정합니다. 대부분의 경우 비활성화 권장 (화면 전체 효과가 중복 적용되어 부자연스러움)")]
        [SerializeField] private bool _renderPostProcessing = false;

        [Header("Environment")]
        [Tooltip("미러에서만 사용할 특별한 스카이박스입니다. 비워두면 메인 카메라의 스카이박스를 사용합니다. (예: 마법 거울의 다른 세계 하늘)")]
        [SerializeField] private Material _customSkybox;

        [Tooltip("오클루전 컬링 사용 여부입니다. 성능 향상에 도움되지만 깜빡임 현상 발생 시 비활성화하세요. 오클루전이 베이크되어 있어야 효과적입니다.")]
        [SerializeField] private bool _useOcclusionCulling = true;

        [Header("Advanced Settings")]
        [Tooltip("미러 렌더링 텍스처 크기입니다. 16의 배수 권장 (GPU 최적화). 높을수록 선명하지만 메모리 사용량 급증")]
        [SerializeField] private Vector2Int _textureSize = new Vector2Int(512, 512);

        [Tooltip("픽셀 라이트 비활성화로 성능을 향상시킵니다. 모바일/VR에서 성능 문제 발생 시 활성화 권장")]
        [SerializeField] private bool _disablePixelLights = false;
    }
}