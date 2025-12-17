#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.VivenComponents.Mirror.Editor
{
    /// <summary>
    /// SDKMirrorSurfaceComponent 전용 Custom Inspector.
    /// - 사이즈/페이드/클리핑/자식 Surface 관리
    /// </summary>
    [CustomEditor(typeof(SDKMirrorSurfaceComponent), true)]
    public class SDKMirrorSurfaceEditor : UnityEditor.Editor
    {
        private SerializedProperty _spMaterial;
        private SerializedProperty _spMaterialIndex;
        private SerializedProperty _spMeshRenderer;
        private SerializedProperty _spForwardTransform;
        
        private SerializedProperty _spMaxRenderingDistance;
        private SerializedProperty _spFadeDistance;
        private SerializedProperty _spFadeColor;
        private SerializedProperty _spMaxBlend;
        
        private SerializedProperty _spUseRecursiveDarkening;
        private SerializedProperty _spClippingPlaneOffset;
        private SerializedProperty _spChildSurfaces;

        private ReorderableList _childList;

        /// <summary>
        /// 에디터 초기화: SerializedProperty 바인딩 및 리스트 구성.
        /// </summary>
        private void OnEnable()
        {
            _spMaterial = serializedObject.FindProperty("_material");
            _spMaterialIndex = serializedObject.FindProperty("_materialIndex");
            _spMeshRenderer = serializedObject.FindProperty("_meshRenderer");
            _spForwardTransform = serializedObject.FindProperty("_forwardTransform");
            
            _spMaxRenderingDistance = serializedObject.FindProperty("_maxRenderingDistance");
            _spFadeDistance = serializedObject.FindProperty("_fadeDistance");
            _spFadeColor = serializedObject.FindProperty("_fadeColor");
            _spMaxBlend = serializedObject.FindProperty("_maxBlend");
            
            _spUseRecursiveDarkening = serializedObject.FindProperty("_useRecursiveDarkening");
            _spClippingPlaneOffset = serializedObject.FindProperty("_clippingPlaneOffset");
            _spChildSurfaces = serializedObject.FindProperty("_childSurfaces");

            _childList = new ReorderableList(serializedObject, _spChildSurfaces, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Child Surfaces"),
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _spChildSurfaces.GetArrayElementAtIndex(index);
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += 2;
                    EditorGUI.PropertyField(rect, element, GUIContent.none);
                }
            };
        }

        /// <summary>
        /// Inspector GUI 렌더링.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var component = (SDKMirrorSurfaceComponent)target;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Surface Settings
            EditorGUILayout.PropertyField(_spMaterial, new GUIContent("Material"));
            EditorGUILayout.PropertyField(_spMaterialIndex, new GUIContent("Material Index"));
            EditorGUILayout.PropertyField(_spMeshRenderer, new GUIContent("Mesh Renderer"));
            EditorGUILayout.PropertyField(_spForwardTransform, new GUIContent("Forward Transform"));

            // Distance & Fade
            EditorGUILayout.PropertyField(_spMaxRenderingDistance, new GUIContent("Max Rendering Distance"));
            EditorGUILayout.Slider(_spFadeDistance, 0f, 1f, new GUIContent("Fade Distance"));
            EditorGUILayout.PropertyField(_spFadeColor, new GUIContent("Fade Color"));
            EditorGUILayout.Slider(_spMaxBlend, 0f, 1f, new GUIContent("Max Blend"));

            // Reflection Behavior
            EditorGUILayout.PropertyField(_spUseRecursiveDarkening, new GUIContent("Use Recursive Darkening"));

            // Advanced
            EditorGUILayout.PropertyField(_spClippingPlaneOffset, new GUIContent("Clipping Plane Offset"));

            // Children
            _childList.DoLayoutList();

            // 경고/가이드
            DrawHintsAndWarnings();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 현재 값에 대한 기본 가이드/경고 출력.
        /// </summary>
        private void DrawHintsAndWarnings()
        {
            float fade = _spFadeDistance.floatValue;
            float dist = _spMaxRenderingDistance.floatValue;
            
            if (fade > 0.9f)
            {
                EditorGUILayout.HelpBox("Fade Distance가 너무 크면 미러가 멀리서 거의 보이지 않을 수 있습니다.", MessageType.Info);
            }
            if (dist > 100f)
            {
                EditorGUILayout.HelpBox("Max Rendering Distance가 과도하면 성능에 영향이 있을 수 있습니다.", MessageType.Info);
            }
        }
    }
}
#endif