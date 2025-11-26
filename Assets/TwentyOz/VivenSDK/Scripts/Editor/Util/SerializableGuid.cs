using System;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Util
{
    [Serializable]
    public struct SerializableGuid : ISerializationCallbackReceiver 
    {
        private Guid guid;
        [SerializeField] private string serializedGuid;

        public SerializableGuid(Guid guid)
        {
            this.guid = guid;
            serializedGuid = guid.ToString();
        }

        // 새 GUID를 생성하는 정적 팩토리 메서드
        public static SerializableGuid NewGuid()
        {
            return new SerializableGuid(Guid.NewGuid());
        }
        
        // 비어있거나 기본값인 GUID인지 체크
        public bool IsEmpty()
        {
            return guid == Guid.Empty;
        }

        public Guid Get() => guid;

        public void OnBeforeSerialize()
        {
            serializedGuid = guid.ToString();
        }

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(serializedGuid) || serializedGuid.Trim() == "00000000-0000-0000-0000-000000000000")
            {
                // 빈 GUID이거나 기본값인 경우 새로운 GUID 생성
                guid = Guid.NewGuid();
                serializedGuid = guid.ToString();
                return;
            }

            if (Guid.TryParse(serializedGuid, out var parsedGuid))
            {
                guid = parsedGuid;
            }
            else
            {
                Debug.LogError($"Failed to parse Guid from string: {serializedGuid}");
                // 파싱 실패 시 새로운 GUID 생성
                guid = Guid.NewGuid();
                serializedGuid = guid.ToString();
            }
        }
    }


    // TODO: UI 수정하기
    /// <summary>
    /// Property drawer for SerializableGuid
    ///
    /// Author: Searous
    /// source: https://discussions.unity.com/t/cannot-serialize-a-guid-field-in-class/489272/8
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        private float _buttonSize;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Start property draw
            EditorGUI.BeginProperty(position, label, property);

            // Get property
            SerializedProperty serializedGuid = property.FindPropertyRelative("serializedGuid");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            _buttonSize = 40; // Update size of buttons to always fit perfectly above the string representation field

            // Buttons
            if (GUI.Button(new Rect(position.xMin, position.yMin, _buttonSize, position.height), "New"))
            {
                serializedGuid.stringValue = Guid.NewGuid().ToString();
            }
            
            // Draw fields - passes GUIContent.none to each so they are drawn without labels
            Rect pos = new Rect(position.xMin + _buttonSize + 10, position.yMin, position.width - _buttonSize - 10, position.height);
            EditorGUI.PropertyField(pos, serializedGuid, GUIContent.none);

            // End property
            EditorGUI.EndProperty();
        }
    }
}