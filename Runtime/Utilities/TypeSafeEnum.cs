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

    public abstract class TypeSafeEnum<T> : TypeSafeEnum where T : TypeSafeEnum<T>
    {
        public static readonly IReadOnlyList<T> Values;

        static TypeSafeEnum()
        {
            Values = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(a => a.PropertyType == typeof(T))
                .Select(a => (T)a.GetValue(null))
                .ToList();
        }

        protected TypeSafeEnum(string name) : base(name) { }
    }
}