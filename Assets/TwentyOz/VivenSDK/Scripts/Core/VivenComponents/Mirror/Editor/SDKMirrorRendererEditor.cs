#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Mirror.Editor
{
    /// <summary>
    /// SDKMirrorRendererComponent 전용 Custom Inspector.
    /// - 자동 최적화 UI, 플랫폼 가이드, 성능 경고
    /// - 미러 품질(Recursions/AA/ScreenScaleFactor/FrameSkip) 및 고급 설정(TextureSize/DisablePixelLights)
    /// </summary>
    [CustomEditor(typeof(SDKMirrorRendererComponent), true)]
    public class SDKMirrorRendererEditor : UnityEditor.Editor
    {
        private SerializedProperty _spMirrorSurfaces;
        private SerializedProperty _spOptimizationMode;

        private SerializedProperty _spRecursions;
        private SerializedProperty _spScreenScaleFactor;
        private SerializedProperty _spAntiAliasingLevel;
        private SerializedProperty _spRenderingEnabled;
        private SerializedProperty _spFrameSkip;

        private SerializedProperty _spRenderLayers;
        private SerializedProperty _spRenderShadows;
        private SerializedProperty _spRenderPostProcessing;
        private SerializedProperty _spCustomSkybox;
        private SerializedProperty _spUseOcclusionCulling;

        private SerializedProperty _spTextureSize;
        private SerializedProperty _spDisablePixelLights;

        /// <summary>
        /// 에디터 초기화: 바인딩할 SerializedProperty 준비.
        /// </summary>
        private void OnEnable()
        {
            _spMirrorSurfaces = serializedObject.FindProperty("_mirrorSurfaces");
            _spOptimizationMode = serializedObject.FindProperty("optimizationMode");

            _spRecursions = serializedObject.FindProperty("_recursions");
            _spScreenScaleFactor = serializedObject.FindProperty("_screenScaleFactor");
            _spAntiAliasingLevel = serializedObject.FindProperty("_antiAliasingLevel");
            _spRenderingEnabled = serializedObject.FindProperty("_renderingEnabled");
            _spFrameSkip = serializedObject.FindProperty("_frameSkip");

            _spRenderLayers = serializedObject.FindProperty("_renderLayers");
            _spRenderShadows = serializedObject.FindProperty("_renderShadows");
            _spRenderPostProcessing = serializedObject.FindProperty("_renderPostProcessing");
            _spCustomSkybox = serializedObject.FindProperty("_customSkybox");
            _spUseOcclusionCulling = serializedObject.FindProperty("_useOcclusionCulling");

            _spTextureSize = serializedObject.FindProperty("_textureSize");
            _spDisablePixelLights = serializedObject.FindProperty("_disablePixelLights");
        }

        /// <summary>
        /// Inspector GUI 렌더링.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var component = (SDKMirrorRendererComponent)target;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Surface Connections
            EditorGUILayout.PropertyField(_spMirrorSurfaces, new GUIContent("Mirror Surfaces"));

            // Optimization
            EditorGUILayout.PropertyField(_spOptimizationMode, new GUIContent("Optimization Mode"));

            var modeIndex = _spOptimizationMode.enumValueIndex;
            if (modeIndex == (int)OptimizationMode.Auto)
            {
                EditorGUILayout.HelpBox("Auto 모드: 실행 플랫폼(모바일/VR/데스크톱) 감지에 따라 권장 설정이 런타임에서 자동 적용됩니다.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("수동 설정 모드입니다. 모든 품질 파라미터를 직접 조정합니다.", MessageType.Info);
            }

            // Mirror Settings - Custom 모드일 때만 표시
            bool isCustomEditable = _spOptimizationMode.enumValueIndex == (int)OptimizationMode.Custom;

            if (isCustomEditable)
            {
                DrawSection("Mirror Quality");
                
                // Recursions (1~8)
                int rec = EditorGUILayout.IntSlider("Recursions", _spRecursions.intValue, 1, 8);
                _spRecursions.intValue = Mathf.Clamp(rec, 1, 8);

                // AA (Popup)
                int aaIdx = ToAAIndex(_spAntiAliasingLevel.intValue);
                int newAaIdx = EditorGUILayout.Popup("Anti-Aliasing", aaIdx, AAOptions);
                _spAntiAliasingLevel.intValue = AAValues[Mathf.Clamp(newAaIdx, 0, AAValues.Length - 1)];

                // Screen Scale Factor (0.1~1.0)
                float ssf = EditorGUILayout.Slider("Screen Scale Factor", _spScreenScaleFactor.floatValue, 0.1f, 1.0f);
                _spScreenScaleFactor.floatValue = Mathf.Clamp(ssf, 0.1f, 1.0f);

                // Rendering Enabled
                EditorGUILayout.PropertyField(_spRenderingEnabled, new GUIContent("Rendering Enabled"));

                // Frame Skip (>=0)
                int fs = EditorGUILayout.IntField("Frame Skip", _spFrameSkip.intValue);
                _spFrameSkip.intValue = Mathf.Max(0, fs);
            }

            // Rendering Settings
            EditorGUILayout.PropertyField(_spRenderLayers, new GUIContent("Render Layers"));
            EditorGUILayout.PropertyField(_spRenderShadows, new GUIContent("Render Shadows"));
            EditorGUILayout.PropertyField(_spRenderPostProcessing, new GUIContent("Render Post Processing"));

            // Environment
            EditorGUILayout.PropertyField(_spCustomSkybox, new GUIContent("Custom Skybox"));
            EditorGUILayout.PropertyField(_spUseOcclusionCulling, new GUIContent("Use Occlusion Culling"));

            // Advanced
            EditorGUILayout.PropertyField(_spTextureSize, new GUIContent("Texture Size"));
            EditorGUILayout.PropertyField(_spDisablePixelLights, new GUIContent("Disable Pixel Lights"));

            // 플랫폼별 권장/경고 안내
            DrawHintsAndWarnings();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// 섹션 헤더를 그린다.
        /// </summary>
        private void DrawSection(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        /// <summary>
        /// 현재 설정에 대한 플랫폼별 권장 및 경고 메시지 출력.
        /// </summary>
        private void DrawHintsAndWarnings()
        {
            string platform = GetCurrentPlatformName();

            // 읽기용 값 스냅샷
            int rec = _spRecursions.intValue;
            int aa = _spAntiAliasingLevel.intValue;
            float ssf = _spScreenScaleFactor.floatValue;
            int fs = _spFrameSkip.intValue;
            var ts = _spTextureSize.vector2IntValue;

            bool isMobile = Application.isMobilePlatform;
            bool isVR = IsVRMode();

            EditorGUILayout.HelpBox($"Platform Hint: {platform}", MessageType.Info);

            if (isMobile)
            {
                if (rec > 3) EditorGUILayout.HelpBox("모바일에서는 Recursions 3 이하를 권장합니다.", MessageType.Warning);
                if (aa > 1) EditorGUILayout.HelpBox("모바일에서는 Anti-Aliasing 'None' 권장(성능).", MessageType.Warning);
                if (ssf > 0.5f) EditorGUILayout.HelpBox("모바일에서는 Screen Scale Factor 0.5 이하를 권장합니다.", MessageType.Warning);
                if (fs < 2) EditorGUILayout.HelpBox("모바일에서는 Frame Skip 2 이상으로 성능 향상을 기대할 수 있습니다.", MessageType.Info);
            }
            else if (isVR)
            {
                if (rec > 2) EditorGUILayout.HelpBox("VR에서는 Recursions 2 이하를 권장합니다.", MessageType.Warning);
                if (aa > 2) EditorGUILayout.HelpBox("VR에서는 Anti-Aliasing 'Low' 이하를 권장합니다.", MessageType.Warning);
                if (ssf > 0.5f) EditorGUILayout.HelpBox("VR 기본 권장은 Screen Scale Factor 0.5 입니다.", MessageType.Info);
                if (fs < 1) EditorGUILayout.HelpBox("VR에서는 Frame Skip 1 이상 설정이 유용할 수 있습니다.", MessageType.Info);
            }
            else
            {
                if (aa > 4) EditorGUILayout.HelpBox("데스크톱에서는 Anti-Aliasing Medium(4) 이하를 우선 검토하세요.", MessageType.Warning);
                if (ssf < 0.5f || ssf > 1.0f) EditorGUILayout.HelpBox("데스크톱 권장 범위: Screen Scale Factor 0.5 ~ 1.0", MessageType.Info);
            }

            if ((ts.x % 16) != 0 || (ts.y % 16) != 0)
            {
                EditorGUILayout.HelpBox("Texture Size는 16의 배수를 권장합니다.", MessageType.Info);
            }
        }

        // Anti-Aliasing 관련 유틸리티
        private static readonly string[] AAOptions = { "None", "Low", "Medium", "High" };
        private static readonly int[] AAValues = { 1, 2, 4, 8 };

        private static int ToAAIndex(int aaValue)
        {
            return aaValue switch
            {
                1 => 0, // None
                2 => 1, // Low
                4 => 2, // Medium
                8 => 3, // High
                _ => 1  // Default to Low
            };
        }

        private static string GetCurrentPlatformName()
        {
            return EditorUserBuildSettings.activeBuildTarget.ToString();
        }

        private static bool IsVRMode()
        {
            // SDK 버전에서는 실제 VR 감지 로직 없이 기본값 반환
            return false;
        }
    }
}
#endif