using UnityEngine;

namespace ED.Additional.Utilities
{
    public static class VectorUtility
    {
        public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
        {
            var AB = b - a;
            var AV = value - a;
            return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
        }
        
        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            var AB = b - a;
            var AV = value - a;
            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }
    }
}