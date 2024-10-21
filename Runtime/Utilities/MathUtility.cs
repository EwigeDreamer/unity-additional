using System;

namespace ED.Additional.Utilities
{
    public static class MathUtility
    {
        public static int Loop(int value, int min, int max) {
            var len = max - min;
            if (len <= 1) return min;
            value -= min;
            var loops = value / len;
            if (value < 0 && value % len != 0) --loops;
            value -= loops * len;
            value += min;
            return value;
        }

        public static float RotateTowards(float from, float to, float maxDegreesDelta)
        {
            var deltaMax = DeltaAngle(from, to);
            var sign = deltaMax > 0f ? 1f : -1f;
            deltaMax *= sign;
            var delta = Math.Min(maxDegreesDelta, deltaMax);
            return from + delta * sign;
        }
        
        public static float DeltaAngle(float from, float to)
        {
            var delta = to - from;
            delta = (delta + 180f) % 360f;
            if (delta < 0) delta += 360f;
            delta -= 180f;
            if (delta == -180f) delta = 180f;
            return delta;
        }
    }
}