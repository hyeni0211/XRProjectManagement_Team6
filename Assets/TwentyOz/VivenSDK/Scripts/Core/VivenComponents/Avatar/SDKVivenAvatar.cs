using System.Collections.Generic;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Avatar
{
    /// <summary>
    /// VivenAvatar를 제작하기 위한 컴포넌트
    /// VivenAvatar로 빌드하기 위해서는 SDKVivenAvatar 컴포넌트가 필요합니다.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("VivenSDK/Avatar/VivenAvatar")]
    public class SDKVivenAvatar : MonoBehaviour
    {
        [SerializeField] private List<GameObject> sdkCharacterCullingObjects;
        [SerializeField] private SkinnedMeshRenderer sdkFace;

        // [SerializeField] private SkinnedMeshRenderer sdkFaceMeshRenderer;
        // [SerializeField] private SkinnedMeshRenderer sdkBodyMeshRenderer;
        // [SerializeField] private SkinnedMeshRenderer sdkHairMeshRenderer;
    }
}