using Twoz.Viven.Interactions;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Interactions
{
    [CustomEditor(typeof(VivenGrabbableModule))]
    public class GrabbableModuleEditor : UnityEditor.Editor
    {
        private bool showAdvanced; // 토글 상태를 저장할 변수

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            var boldItalic = new GUIStyle(EditorStyles.boldLabel) { fontStyle = FontStyle.BoldAndItalic };
            EditorGUILayout.LabelField("Grab Type", boldItalic);

            var grabType = serializedObject.FindProperty("grabType");
            EditorGUILayout.PropertyField(grabType, new GUIContent("Object Grab Type"));

            var parentToHandOnGrab = serializedObject.FindProperty("parentToHandOnGrab");
            EditorGUILayout.PropertyField(parentToHandOnGrab, new GUIContent("Parent To Hand On Grab"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Interaction", boldItalic);

            var holdTimeThreshold = serializedObject.FindProperty("holdTimeThreshold");
            EditorGUILayout.PropertyField(holdTimeThreshold, new GUIContent("Hold Time Threshold"));

            var throwForce = serializedObject.FindProperty("throwForce");
            EditorGUILayout.PropertyField(throwForce, new GUIContent("Viven Throw Force"));

            EditorGUILayout.Space();

            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced");

            if (showAdvanced)
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Grab Point", boldItalic);

                var grabPointsPC = serializedObject.FindProperty("grabPoints");
                EditorGUILayout.PropertyField(grabPointsPC, new GUIContent("Grab Points"));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Attach", boldItalic);

                var attachPoints = serializedObject.FindProperty("attachPoints");
                EditorGUILayout.PropertyField(attachPoints, new GUIContent("Viven Attach Points"));

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Layer", boldItalic);

                var excludeLayerObjects = serializedObject.FindProperty("excludeLayerObjects");
                EditorGUILayout.PropertyField(excludeLayerObjects, new GUIContent("Exclude Layer Game Objects"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}