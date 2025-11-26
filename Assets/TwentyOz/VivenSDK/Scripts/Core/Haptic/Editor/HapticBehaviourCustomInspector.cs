using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Core.Haptic.Editor
{
    [CustomEditor(typeof(HapticBehaviour))]
    public class HapticBehaviourCustomInspector : UnityEditor.Editor
    {
        private SerializedProperty Hardness            => serializedObject.FindProperty(nameof(HapticBehaviour.hardness));
        private SerializedProperty Smoothness          => serializedObject.FindProperty(nameof(HapticBehaviour.smoothness));
        private SerializedProperty Warmness            => serializedObject.FindProperty(nameof(HapticBehaviour.warmness));
        private SerializedProperty Friction            => serializedObject.FindProperty(nameof(HapticBehaviour.friction));
        private SerializedProperty AutoPopulateOnAwake => serializedObject.FindProperty(nameof(HapticBehaviour.autoPopulateOnAwake));
        private SerializedProperty HapticIntensity     => serializedObject.FindProperty(nameof(HapticBehaviour.hapticIntensity));

        private MeshRenderer _meshRenderer;
        private Collider     _collider;

        private void OnEnable()
        {
            _meshRenderer = ((HapticBehaviour)target).GetComponent<MeshRenderer>();
            _collider     = ((HapticBehaviour)target).GetComponent<Collider>();
        }

        public override void OnInspectorGUI()
        {
            var headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
            EditorGUILayout.LabelField("Viven Haptic Behaviour", headerStyle);

            if (_meshRenderer == null)
                EditorGUILayout.HelpBox("MeshRenderer 컴포넌트가 없습니다.", MessageType.Error);
            else if (_meshRenderer.sharedMaterial == null)
                EditorGUILayout.HelpBox("MeshRenderer 컴포넌트의 Material이 없습니다.", MessageType.Error);
            else if (_meshRenderer.sharedMaterial.shader.name != "Standard")
                EditorGUILayout.HelpBox("MeshRenderer 컴포넌트의 Material의 Shader가 Standard가 아닙니다.", MessageType.Error);
            else
                EditorGUILayout.HelpBox("Haptic 장비 연동을 위한 컴포넌트입니다.", MessageType.Info);


            EditorGUILayout.BeginVertical();

            Slider(EditorGUILayout.GetControlRect(), Hardness, 0, 1, new GUIContent("Hardness"));
            Slider(EditorGUILayout.GetControlRect(), Smoothness, 0, 1, new GUIContent("Smoothness"));
            Slider(EditorGUILayout.GetControlRect(), Warmness, 0, 1, new GUIContent("Warmness"));
            if(_collider == null)
                EditorGUILayout.HelpBox("Collider 컴포넌트가 없습니다.", MessageType.Error);
            if(_collider != null && _collider.material == null)
                EditorGUILayout.HelpBox("Collider 컴포넌트의 Material이 없습니다.", MessageType.Error);
            Slider(EditorGUILayout.GetControlRect(), Friction, 0, 1, new GUIContent("Friction"));
            Slider(EditorGUILayout.GetControlRect(), HapticIntensity, 0, 1, new GUIContent("Haptic Intensity"));
            EditorGUILayout.PropertyField(AutoPopulateOnAwake);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Populate"))
            {
                Hardness.floatValue   = _meshRenderer.sharedMaterial.GetFloat("_Glossiness");
                Smoothness.floatValue = _meshRenderer.sharedMaterial.GetFloat("_Glossiness");
                Warmness.floatValue   = _meshRenderer.sharedMaterial.GetFloat("_Metallic");
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        void Slider(Rect position, SerializedProperty property, float leftValue, float rightValue, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.Slider(position, label, property.floatValue, leftValue, rightValue);

            if (EditorGUI.EndChangeCheck()) property.floatValue = newValue;

            EditorGUI.EndProperty();
        }
    }
}