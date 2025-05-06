using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace ED.Additional.Utilities
{
    [System.Serializable]
    public abstract class TypeSafeEnum
    {
        [SerializeField] private string _name;
        
        protected TypeSafeEnum(string name) => _name = name ?? string.Empty;

        public override string ToString() => _name;

        public static implicit operator string(TypeSafeEnum value) => value._name;

        private bool Equals(TypeSafeEnum other) => string.Equals(_name, other._name);

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((TypeSafeEnum)other);
        }

        public override int GetHashCode() => _name.GetHashCode();

        public static bool operator ==(TypeSafeEnum left, TypeSafeEnum right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return !ReferenceEquals(right, null) && left.Equals(right);
        }

        public static bool operator !=(TypeSafeEnum left, TypeSafeEnum right) => !(left == right);
        
#if UNITY_EDITOR
        protected abstract class PropertyDrawer<TSelf> : BasePropertyDrawer<TSelf> where TSelf : TypeSafeEnum
        {
            protected override object[] GetParameters(TSelf member) => new object[] { member._name };
        }
#endif
    }

    [System.Serializable]
    public abstract class TypeSafeEnum<TSelf> : TypeSafeEnum where TSelf : TypeSafeEnum<TSelf>
    {
        public static readonly IReadOnlyList<TSelf> Values = TypeSafeEnumUtility.GetMembers<TSelf>();

        protected TypeSafeEnum(string name) : base(name) { }
    }

    [System.Serializable]
    public abstract class TypeSafeValueEnum<TValue>
    {
        [SerializeField] private string _name;
        [SerializeField] private TValue _value;
        
        protected TypeSafeValueEnum(string name, TValue value)
        {
            _name = name ?? string.Empty;
            _value = value;
        }

        public override string ToString() => _name;

        public static implicit operator string(TypeSafeValueEnum<TValue> value) => value._name;
        public static implicit operator TValue(TypeSafeValueEnum<TValue> value) => value._value;

        private bool Equals(TypeSafeValueEnum<TValue> other) => _name.Equals(other._name) && _value.Equals(other._value);

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((TypeSafeValueEnum<TValue>)other);
        }

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(TypeSafeValueEnum<TValue> left, TypeSafeValueEnum<TValue> right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return !ReferenceEquals(right, null) && left.Equals(right);
        }

        public static bool operator !=(TypeSafeValueEnum<TValue> left, TypeSafeValueEnum<TValue> right) => !(left == right);
        
#if UNITY_EDITOR
        protected abstract class PropertyDrawer<TSelf> : BasePropertyDrawer<TSelf> where TSelf : TypeSafeValueEnum<TValue>
        {
            protected override object[] GetParameters(TSelf member) => new object[] { member._name, member._value };
        }
#endif
        
    }

    [System.Serializable]
    public abstract class TypeSafeValueEnum<TValue, TSelf> : TypeSafeValueEnum<TValue> where TSelf : TypeSafeValueEnum<TValue, TSelf>
    {
        public static readonly IReadOnlyList<TSelf> Values = TypeSafeEnumUtility.GetMembers<TSelf>();

        protected TypeSafeValueEnum(string name, TValue value) : base(name, value) { }
    }
    
#if UNITY_EDITOR
    public abstract class BasePropertyDrawer<TSelf> : PropertyDrawer
    {
        private static readonly GUIContent[] Names = TypeSafeEnumUtility.GetNames<TSelf>().Select(a => new GUIContent(a)).ToArray();
        private static readonly List<TSelf> Members = TypeSafeEnumUtility.GetMembers<TSelf>().ToList();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (Check((TSelf)property.boxedValue)) return EditorGUIUtility.singleLineHeight;
            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!Check((TSelf)property.boxedValue))
            {
                position.GetRows(EditorGUIUtility.standardVerticalSpacing, out var boxPosition, out position);
                EditorGUI.HelpBox(boxPosition, $"Value '{label.text}' is invalid!", MessageType.Error);
            }

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var index = Members.FindIndex(a => a.Equals((TSelf)property.boxedValue));
            index = EditorGUI.Popup(position, label, index, Names);
            if (EditorGUI.EndChangeCheck())
            {
                var member = Members[index];
                var clone = Activator.CreateInstance(
                    typeof(TSelf),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, GetParameters(member), null, null);
                property.boxedValue = clone;
            }

            EditorGUI.EndProperty();
        }

        private bool Check(TSelf value)
        {
            if (value == null) return false;
            var index = Members.FindIndex(a => a.Equals(value));
            if (index < 0) return false;
            return value.Equals(Members[index]);
        }

        protected abstract object[] GetParameters(TSelf member);
    }
#endif

    internal static class TypeSafeEnumUtility
    {
        public static string[] GetNames<T>()
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(a => a.FieldType == typeof(T))
                .Select(a => a.Name);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(a => a.PropertyType == typeof(T) && a.CanRead && a.GetIndexParameters().Length == 0)
                .Select(a => a.Name);
            return fields.Concat(properties).ToArray();
        }
        
        public static T[] GetMembers<T>()
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(a => a.FieldType == typeof(T))
                .Select(a => (T)a.GetValue(null));
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(a => a.PropertyType == typeof(T) && a.CanRead && a.GetIndexParameters().Length == 0)
                .Select(a => (T)a.GetValue(null));
            return fields.Concat(properties).ToArray();
        }
        
        public static void GetRows(this Rect source, float spacing, out Rect rect1, out Rect rect2)
        {
            float l = (source.height - spacing) / 2f;
            float lsp = l + spacing;
            rect1 = source;
            rect1.yMax -= lsp;

            rect2 = source;
            rect2.yMin += lsp;
        }
    }
}