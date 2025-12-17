using TwentyOz.VivenSDK.Scripts.Core.Lua;
using UnityEditor;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Lua
{
    public abstract class BaseValueCustomPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var nameProperty = property.FindPropertyRelative("name");
            var valueProperty = property.FindPropertyRelative("value");

            // Draw the name field
            Rect nameRect = new Rect(position.x, position.y, position.width * 0.4f, position.height);
            EditorGUI.PropertyField(nameRect, nameProperty, GUIContent.none);
            // Draw the value field
            Rect valueRect = new Rect(position.x + position.width * 0.4f + 20, position.y, position.width * 0.6f - 50,
                position.height);
            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ObjectValue))]
    public class ObjectValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(GameObjectValue))]
    public class GameObjectValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(Vector3Value))]
    public class Vector3ValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(FloatValue))]
    public class FloatValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(IntValue))]
    public class IntValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(BoolValue))]
    public class BoolValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(StringValue))]
    public class StringValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(ColorValue))]
    public class ColorValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(VivenScriptValue))]
    public class VivenScriptValueCustomPropertyDrawer : BaseValueCustomPropertyDrawer
    {
    }
}