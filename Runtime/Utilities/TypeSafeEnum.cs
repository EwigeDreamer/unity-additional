using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ED.Additional.Utilities
{
    public abstract class TypeSafeEnum
    {
        private readonly string _name;

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
            return left.Equals(right);
        }

        public static bool operator !=(TypeSafeEnum left, TypeSafeEnum right) => !(left == right);
    }

    public abstract class TypeSafeEnum<TSelf> : TypeSafeEnum where TSelf : TypeSafeEnum<TSelf>
    {
        public static readonly IReadOnlyList<TSelf> Values;

        static TypeSafeEnum()
        {
            Values = typeof(TSelf).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(a => a.PropertyType == typeof(TSelf))
                .Select(a => (TSelf)a.GetValue(null))
                .ToList();
        }

        protected TypeSafeEnum(string name) : base(name) { }
    }

    public abstract class TypeSafeValueEnum<TValue>
    {
        private readonly string _name;
        private readonly TValue _value;

        protected TypeSafeValueEnum(string name, TValue value)
        {
            _name = name ?? string.Empty;
            _value = value;
        }

        public override string ToString() => _name;

        public static implicit operator string(TypeSafeValueEnum<TValue> value) => value._name;
        public static implicit operator TValue(TypeSafeValueEnum<TValue> value) => value._value;

        private bool Equals(TypeSafeValueEnum<TValue> other) => _value.Equals(other._value);

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
            return left.Equals(right);
        }

        public static bool operator !=(TypeSafeValueEnum<TValue> left, TypeSafeValueEnum<TValue> right) => !(left == right);
    }

    public abstract class TypeSafeValueEnum<TValue, TSelf> : TypeSafeValueEnum<TValue> where TSelf : TypeSafeValueEnum<TValue, TSelf>
    {
        public static readonly IReadOnlyList<TSelf> Values;

        static TypeSafeValueEnum()
        {
            Values = typeof(TSelf).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(a => a.PropertyType == typeof(TSelf))
                .Select(a => (TSelf)a.GetValue(null))
                .ToList();
        }

        protected TypeSafeValueEnum(string name, TValue value) : base(name, value) { }
    }
}