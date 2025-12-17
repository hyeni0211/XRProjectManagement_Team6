using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Mirror
{
    /// <summary>
/// VIVEN 환경에서 Mirrors and Reflections for VR의 MirrorSurface를 래핑하는 컴포넌트입니다.
///
/// - 목적: 외부 패키지 MirrorSurface를 안전하게 감싸고 VIVEN 규칙(로깅/예외/최적화)을 적용합니다.
/// - 기능: Transform 기반 크기 제어, 페이드 효과/거리, 클리핑 평면, 자식 Surface 관리.
///
/// 사용 예(C#):
/// <example>
/// <code>
/// var ms = gameObject.GetComponent<SDKMirrorSurfaceComponent>();
/// ms.SetSize(3.0f, 2.0f);
/// ms.SetFadeColor(Color.red);
/// ms.SetMaxRenderingDistance(15.0f);
/// </code>
/// </example>
///
/// 사용 예(Lua, xLua):
/// <example>
/// <code>
/// local ms = gameObject:GetComponent("SDKMirrorSurfaceComponent")
/// if ms ~= nil then
///   ms:SetSize(3.0, 2.0)
///   ms:SetFadeColor(Color.red)
///   ms:SetMaxRenderingDistance(15.0)
/// end
/// </code>
/// </example>
/// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("VivenSDK/Mirror/SDKMirrorSurfaceComponent")]
    public class SDKMirrorSurfaceComponent : MonoBehaviour
    {
        [Header("Surface Settings")]
        [Tooltip("미러 표면에 사용할 재질입니다. 미러 셰이더가 적용된 Material을 지정하세요. 비워두면 MeshRenderer의 첫 번째 재질을 사용합니다.")]
        [SerializeField] private Material _material;

        [Tooltip("MeshRenderer의 재질 배열에서 미러로 사용할 재질의 인덱스입니다. (0=첫 번째 재질)")]
        [SerializeField] private int _materialIndex = 0;

        [Tooltip("미러 표면을 렌더링할 MeshRenderer입니다. 비워두면 이 GameObject의 MeshRenderer를 자동으로 사용합니다.")]
        [SerializeField] private MeshRenderer _meshRenderer;

        [Tooltip("미러의 앞면 방향을 결정하는 Transform입니다. 비워두면 이 GameObject의 Transform을 사용합니다. Z축이 미러의 정면 방향입니다.")]
        [SerializeField] private Transform _forwardTransform;

        [Header("Distance & Fade")]
        [Tooltip("미러가 렌더링되는 최대 거리입니다. 이 거리를 벗어나면 미러 업데이트가 중단되어 성능이 향상됩니다.")]
        [Min(0)]
        [SerializeField] private float _maxRenderingDistance = 10.0f;

        [Tooltip("페이드가 시작되는 거리 비율 (0~1). 최대 거리의 이 비율 지점부터 서서히 어두워집니다. (예: 0.5 = 최대 거리의 50% 지점부터 페이드)")]
        [Range(0f, 1f)]
        [SerializeField] private float _fadeDistance = 0.5f;

        [Tooltip("페이드될 때 혼합되는 색상입니다. 멀어질수록 이 색상으로 변해갑니다.")]
        [SerializeField] private Color _fadeColor = Color.black;

        [Tooltip("거리가 가까울 때 반사가 얼마나 선명하게 보일지 설정합니다. (0~1, 1=완전 반사, 0=투명)")]
        [Range(0f, 1f)]
        [SerializeField] private float _maxBlend = 1.0f;

        [Header("Reflection Behavior")]
        [Tooltip("재귀 반사 시 각 단계마다 어둡게 만들지 설정합니다. 활성화 시 무한 반사가 자연스럽게 감소합니다.")]
        [SerializeField] private bool _useRecursiveDarkening = true;

        [Header("Advanced Settings")]
        [Tooltip("클리핑 평면의 오프셋입니다. 미러 근처 객체가 잘못 렌더링될 때 조정하세요.")]
        [SerializeField] private float _clippingPlaneOffset = 0.0f;

        [Tooltip("같은 평면에 있는 하위 미러 표면들입니다. 이들은 이 미러와 동일한 렌더링 텍스처를 공유하여 성능을 절약합니다.")]
        [SerializeField] private List<SDKMirrorSurfaceComponent> _childSurfaces = new();
    }
}