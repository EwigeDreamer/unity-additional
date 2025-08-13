using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ED.Additional.Utilities
{
    public static class GizmosUtility
    {
        public static void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null, bool crosshair = false)
        {
            using var h1 = BeginLocation(position, rotation, scale);
            if (color.HasValue)
            {
                using var h2 = BeginColor(color.Value);
                DrawWireCube_Internal(crosshair);
            }
            else DrawWireCube_Internal(crosshair);
        }

        public static void DrawWireCircle(Vector3 position, Vector3 normal, float radius, Color? color = null, bool crosshair = false)
        {
            var rotation = Quaternion.LookRotation(normal);
            var scale = Vector3.one * radius * 2f;
            DrawWireCircle(position, rotation, scale, color, crosshair);
        }
        
        public static void DrawWireCircle(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null, bool crosshair = false)
        {
            using var h1 = BeginLocation(position, rotation, scale);
            if (color.HasValue)
            {
                using var h2 = BeginColor(color.Value);
                DrawWireCircle_Internal(crosshair);
            }
            else DrawWireCircle_Internal(crosshair);
        }
        
        public static void DrawWireSphere(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null, bool crosshair = false)
        {
            using var h1 = BeginLocation(position, rotation, scale);
            if (color.HasValue)
            {
                using var h2 = BeginColor(color.Value);
                DrawWireSphere_Internal(crosshair);
            }
            else DrawWireSphere_Internal(crosshair);
        }

        public static void DrawWireArrow(Vector3 start, Vector3 end, float thicness, Color? color = null)
        {
            var path = end - start;
            var position = Vector3.LerpUnclamped(start, end, 0.5f);
            var rotation = Quaternion.LookRotation(path);
            var scale = new Vector3(thicness, thicness, path.magnitude);
            DrawWireArrow(position, rotation, scale, color);
        }
        
        public static void DrawWireArrow(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null)
        {
            using var h1 = BeginLocation(position, rotation, scale);
            if (color.HasValue)
            {
                using var h2 = BeginColor(color.Value);
                DrawArrow_Internal();
            }
            else DrawArrow_Internal();
        }

        public static void DrawSphereSpring(Vector3 start, Vector3 end, float thicness, Color? color = null)
        {
            var path = end - start;
            var position = Vector3.LerpUnclamped(start, end, 0.5f);
            var rotation = Quaternion.LookRotation(path);
            var scale = new Vector3(thicness, thicness, path.magnitude);
            DrawSphereSpring(position, rotation, scale, color);
        }
        
        public static void DrawSphereSpring(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null)
        {
            using var h1 = BeginLocation(position, rotation, scale);
            if (color.HasValue)
            {
                using var h2 = BeginColor(color.Value);
                DrawSphereSpring_Internal();
            }
            else DrawSphereSpring_Internal();
        }

        public static void DrawCylinderSpring(Vector3 start, Vector3 end, float thicness, Color? color = null)
        {
            var path = end - start;
            var position = Vector3.LerpUnclamped(start, end, 0.5f);
            var rotation = Quaternion.LookRotation(path);
            var scale = new Vector3(thicness, thicness, path.magnitude);
            DrawCylinderSpring(position, rotation, scale, color);
        }
        
        public static void DrawCylinderSpring(Vector3 position, Quaternion rotation, Vector3 scale, Color? color = null)
        {
            using var h1 = BeginLocation(position, rotation, scale);
            if (color.HasValue)
            {
                using var h2 = BeginColor(color.Value);
                DrawCylinderSpring_Internal();
            }
            else DrawCylinderSpring_Internal();
        }

#region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawWireCube_Internal(bool crosshair)
        {
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            if (crosshair)
            {
                Gizmos.DrawLine(_crosshairPoints[0], _crosshairPoints[1]);
                Gizmos.DrawLine(_crosshairPoints[2], _crosshairPoints[3]);
                Gizmos.DrawLine(_crosshairPoints[4], _crosshairPoints[5]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawWireCircle_Internal(bool crosshair)
        {
            for (int i = 1; i < _circlePoints.Length; i++)
                Gizmos.DrawLine(_circlePoints[i - 1], _circlePoints[i]);
            if (crosshair)
            {
                Gizmos.DrawLine(_crosshairPoints[0], _crosshairPoints[1]);
                Gizmos.DrawLine(_crosshairPoints[2], _crosshairPoints[3]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawWireSphere_Internal(bool crosshair)
        {
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
            if (crosshair)
            {
                Gizmos.DrawLine(_crosshairPoints[0], _crosshairPoints[1]);
                Gizmos.DrawLine(_crosshairPoints[2], _crosshairPoints[3]);
                Gizmos.DrawLine(_crosshairPoints[4], _crosshairPoints[5]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawArrow_Internal()
        {
            for (int i = 1; i < _arrowPoints.Length; i++)
                Gizmos.DrawLine(_arrowPoints[i - 1], _arrowPoints[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawSphereSpring_Internal()
        {
            for (int i = 1; i < _sphereSpringPoints.Length; i++)
                Gizmos.DrawLine(_sphereSpringPoints[i - 1], _sphereSpringPoints[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawCylinderSpring_Internal()
        {
            for (int i = 1; i < _cylinderSpringPoints.Length; i++)
                Gizmos.DrawLine(_cylinderSpringPoints[i - 1], _cylinderSpringPoints[i]);
        }

#endregion Internal
        
        public static ColorHandler BeginColor(Color color) => new(color);
        public static MatrixHandler BeginMatrix(Matrix4x4 matrix) => new(matrix);
        
        public static MatrixHandler BeginLocation(
            Vector3? position = null,
            Quaternion? rotation = null,
            Vector3? scale = null)
            => BeginMatrix(Matrix4x4.TRS(
                position ?? Vector3.zero,
                rotation ?? Quaternion.identity,
                scale ?? Vector3.one));
        
        public struct ColorHandler : IDisposable
        {
            private readonly Color _colorTmp;

            public ColorHandler(Color color)
            {
                _colorTmp = Gizmos.color;
                Gizmos.color = color;
            }

            public void Dispose()
            {
                Gizmos.color = _colorTmp;
            }
        }
        
        public struct MatrixHandler : IDisposable
        {
            private readonly Matrix4x4 _matrixTmp;

            public MatrixHandler(Matrix4x4 matrix)
            {
                _matrixTmp = Gizmos.matrix;
                Gizmos.matrix = matrix;
            }

            public void Dispose()
            {
                Gizmos.matrix = _matrixTmp;
            }
        }
        
        private static readonly Vector3[] _cubePoints =
        {
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.51f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
        };
        
        private static readonly Vector3[] _circlePoints =
        {
            new Vector3(0.500000f, 0.000000f, 0.000000f),
            new Vector3(0.490393f, 0.097545f, 0.000000f),
            new Vector3(0.461940f, 0.191342f, 0.000000f),
            new Vector3(0.415735f, 0.277785f, 0.000000f),
            new Vector3(0.353553f, 0.353553f, 0.000000f),
            new Vector3(0.277785f, 0.415735f, 0.000000f),
            new Vector3(0.191342f, 0.461940f, 0.000000f),
            new Vector3(0.097545f, 0.490393f, 0.000000f),
            new Vector3(0.000000f, 0.500000f, 0.000000f),
            new Vector3(-0.097545f, 0.490393f, 0.000000f),
            new Vector3(-0.191342f, 0.461940f, 0.000000f),
            new Vector3(-0.277785f, 0.415735f, 0.000000f),
            new Vector3(-0.353553f, 0.353553f, 0.000000f),
            new Vector3(-0.415735f, 0.277785f, 0.000000f),
            new Vector3(-0.461940f, 0.191342f, 0.000000f),
            new Vector3(-0.490393f, 0.097545f, 0.000000f),
            new Vector3(-0.500000f, 0.000000f, 0.000000f),
            new Vector3(-0.490393f, -0.097545f, 0.000000f),
            new Vector3(-0.461940f, -0.191342f, 0.000000f),
            new Vector3(-0.415735f, -0.277785f, 0.000000f),
            new Vector3(-0.353554f, -0.353553f, 0.000000f),
            new Vector3(-0.277785f, -0.415735f, 0.000000f),
            new Vector3(-0.191342f, -0.461940f, 0.000000f),
            new Vector3(-0.097545f, -0.490393f, 0.000000f),
            new Vector3(0.000000f, -0.500000f, 0.000000f),
            new Vector3(0.097545f, -0.490393f, 0.000000f),
            new Vector3(0.191342f, -0.461940f, 0.000000f),
            new Vector3(0.277785f, -0.415735f, 0.000000f),
            new Vector3(0.353553f, -0.353553f, 0.000000f),
            new Vector3(0.415735f, -0.277785f, 0.000000f),
            new Vector3(0.461940f, -0.191342f, 0.000000f),
            new Vector3(0.490393f, -0.097545f, 0.000000f),
            new Vector3(0.500000f, 0.000000f, 0.000000f),
        };
        
        private static readonly Vector3[] _crosshairPoints =
        {
            new Vector3(-0.5f, 0f, 0f),
            new Vector3(0.5f, 0f, 0f),
            new Vector3(0f, -0.5f, 0f),
            new Vector3(0f, 0.5f, 0f),
            new Vector3(0f, 0f, -0.5f),
            new Vector3(0f, 0f, 0.5f),
        };

        private static readonly Vector3[] _arrowPoints =
        {
            new Vector3(0f, 0f, 0.5f),
            new Vector3(0.5f, 0f, 0f),
            new Vector3(0.25f, 0f, 0f),
            new Vector3(0.25f, 0f, -0.5f),
            new Vector3(-0.25f, 0f, -0.5f),
            new Vector3(-0.25f, 0f, 0f),
            new Vector3(-0.5f, 0f, 0f),
            new Vector3(0f, 0f, 0.5f),
            new Vector3(0f, 0.5f, 0f),
            new Vector3(0f, 0.25f, 0f),
            new Vector3(0f, 0.25f, -0.5f),
            new Vector3(0f, -0.25f, -0.5f),
            new Vector3(0f, -0.25f, 0f),
            new Vector3(0f, -0.5f, 0f),
            new Vector3(0f, 0f, 0.5f),
        };

        private static readonly Vector3[] _sphereSpringPoints =
        {
            new Vector3(0.000000f, 0.000000f, 0.500000f),
            new Vector3(0.014168f, 0.008180f, 0.499732f),
            new Vector3(0.016351f, 0.028320f, 0.498930f),
            new Vector3(0.000000f, 0.049009f, 0.497592f),
            new Vector3(-0.032632f, 0.056520f, 0.495722f),
            new Vector3(-0.070536f, 0.040724f, 0.493322f),
            new Vector3(-0.097545f, 0.000000f, 0.490393f),
            new Vector3(-0.098327f, -0.056769f, 0.486939f),
            new Vector3(-0.064705f, -0.112072f, 0.482963f),
            new Vector3(0.000000f, -0.145142f, 0.478470f),
            new Vector3(0.080360f, -0.139187f, 0.473465f),
            new Vector3(0.152529f, -0.088063f, 0.467953f),
            new Vector3(0.191342f, 0.000000f, 0.461940f),
            new Vector3(0.178708f, 0.103177f, 0.455432f),
            new Vector3(0.110572f, 0.191517f, 0.448436f),
            new Vector3(0.000000f, 0.235698f, 0.440961f),
            new Vector3(-0.125000f, 0.216506f, 0.433013f),
            new Vector3(-0.228660f, 0.132017f, 0.424601f),
            new Vector3(-0.277785f, 0.000000f, 0.415735f),
            new Vector3(-0.252220f, -0.145619f, 0.406423f),
            new Vector3(-0.152191f, -0.263601f, 0.396677f),
            new Vector3(0.000000f, -0.317197f, 0.386505f),
            new Vector3(0.164836f, -0.285505f, 0.375920f),
            new Vector3(0.296004f, -0.170899f, 0.364932f),
            new Vector3(0.353553f, -0.000001f, 0.353553f),
            new Vector3(0.316041f, 0.182466f, 0.341796f),
            new Vector3(0.187961f, 0.325556f, 0.329673f),
            new Vector3(0.000001f, 0.386505f, 0.317197f),
            new Vector3(-0.198338f, 0.343532f, 0.304381f),
            new Vector3(-0.351973f, 0.203212f, 0.291239f),
            new Vector3(-0.415735f, 0.000001f, 0.277785f),
            new Vector3(-0.367716f, -0.212300f, 0.264034f),
            new Vector3(-0.216507f, -0.375000f, 0.250000f),
            new Vector3(0.000000f, -0.440961f, 0.235698f),
            new Vector3(0.224218f, -0.388357f, 0.221144f),
            new Vector3(0.394416f, -0.227716f, 0.206354f),
            new Vector3(0.461940f, 0.000001f, 0.191342f),
            new Vector3(0.405259f, 0.233978f, 0.176125f),
            new Vector3(0.236731f, 0.410034f, 0.160720f),
            new Vector3(-0.000002f, 0.478470f, 0.145142f),
            new Vector3(-0.241484f, 0.418257f, 0.129409f),
            new Vector3(-0.421703f, 0.243467f, 0.113538f),
            new Vector3(-0.490393f, -0.000003f, 0.097545f),
            new Vector3(-0.427227f, -0.246664f, 0.081447f),
            new Vector3(-0.247858f, -0.429310f, 0.065263f),
            new Vector3(0.000005f, -0.497592f, 0.049008f),
            new Vector3(0.249469f, -0.432083f, 0.032701f),
            new Vector3(0.432784f, -0.249861f, 0.016359f),
            new Vector3(0.500000f, 0.000006f, 0.000000f),
            new Vector3(0.432778f, 0.249872f, -0.016360f),
            new Vector3(0.249459f, 0.432089f, -0.032702f),
            new Vector3(-0.000007f, 0.497592f, -0.049009f),
            new Vector3(-0.247868f, 0.429304f, -0.065264f),
            new Vector3(-0.427233f, 0.246654f, -0.081448f),
            new Vector3(-0.490393f, -0.000008f, -0.097546f),
            new Vector3(-0.421697f, -0.243477f, -0.113539f),
            new Vector3(-0.241474f, -0.418263f, -0.129410f),
            new Vector3(0.000009f, -0.478470f, -0.145143f),
            new Vector3(0.236741f, -0.410028f, -0.160720f),
            new Vector3(0.405264f, -0.233968f, -0.176126f),
            new Vector3(0.461940f, 0.000010f, -0.191342f),
            new Vector3(0.394410f, 0.227725f, -0.206354f),
            new Vector3(0.224209f, 0.388362f, -0.221145f),
            new Vector3(-0.000009f, 0.440960f, -0.235699f),
            new Vector3(-0.216514f, 0.374995f, -0.250001f),
            new Vector3(-0.367719f, 0.212293f, -0.264034f),
            new Vector3(-0.415735f, -0.000007f, -0.277786f),
            new Vector3(-0.351969f, -0.203217f, -0.291239f),
            new Vector3(-0.198333f, -0.343535f, -0.304381f),
            new Vector3(0.000006f, -0.386505f, -0.317197f),
            new Vector3(0.187964f, -0.325553f, -0.329673f),
            new Vector3(0.316043f, -0.182462f, -0.341796f),
            new Vector3(0.353553f, 0.000004f, -0.353554f),
            new Vector3(0.296002f, 0.170901f, -0.364932f),
            new Vector3(0.164834f, 0.285507f, -0.375920f),
            new Vector3(-0.000003f, 0.317196f, -0.386505f),
            new Vector3(-0.152192f, 0.263600f, -0.396677f),
            new Vector3(-0.252221f, 0.145618f, -0.406423f),
            new Vector3(-0.277785f, -0.000001f, -0.415735f),
            new Vector3(-0.228660f, -0.132018f, -0.424601f),
            new Vector3(-0.124999f, -0.216507f, -0.433013f),
            new Vector3(0.000000f, -0.235698f, -0.440961f),
            new Vector3(0.110572f, -0.191517f, -0.448436f),
            new Vector3(0.178707f, -0.103177f, -0.455432f),
            new Vector3(0.191342f, 0.000000f, -0.461940f),
            new Vector3(0.152529f, 0.088062f, -0.467953f),
            new Vector3(0.080360f, 0.139187f, -0.473465f),
            new Vector3(0.000001f, 0.145143f, -0.478470f),
            new Vector3(-0.064704f, 0.112072f, -0.482963f),
            new Vector3(-0.098327f, 0.056770f, -0.486938f),
            new Vector3(-0.097545f, 0.000001f, -0.490393f),
            new Vector3(-0.070536f, -0.040723f, -0.493322f),
            new Vector3(-0.032632f, -0.056519f, -0.495722f),
            new Vector3(-0.000001f, -0.049009f, -0.497592f),
            new Vector3(0.016351f, -0.028321f, -0.498929f),
            new Vector3(0.014168f, -0.008180f, -0.499732f),
            new Vector3(0.000000f, 0.000000f, -0.500000f),
        };

        private static readonly Vector3[] _cylinderSpringPoints =
        {
            new Vector3(0.500000f, 0.000000f, -0.500000f),
            new Vector3(0.433013f, 0.250000f, -0.479167f),
            new Vector3(0.250000f, 0.433013f, -0.458333f),
            new Vector3(0.000000f, 0.500000f, -0.437500f),
            new Vector3(-0.250000f, 0.433013f, -0.416667f),
            new Vector3(-0.433013f, 0.250000f, -0.395833f),
            new Vector3(-0.500000f, 0.000000f, -0.375000f),
            new Vector3(-0.433013f, -0.250000f, -0.354167f),
            new Vector3(-0.250000f, -0.433013f, -0.333333f),
            new Vector3(0.000000f, -0.500000f, -0.312500f),
            new Vector3(0.250000f, -0.433013f, -0.291667f),
            new Vector3(0.433013f, -0.250000f, -0.270833f),
            new Vector3(0.500000f, 0.000000f, -0.250000f),
            new Vector3(0.433013f, 0.250000f, -0.229167f),
            new Vector3(0.250000f, 0.433013f, -0.208333f),
            new Vector3(0.000000f, 0.500000f, -0.187500f),
            new Vector3(-0.250000f, 0.433013f, -0.166667f),
            new Vector3(-0.433013f, 0.250000f, -0.145833f),
            new Vector3(-0.500000f, 0.000000f, -0.125000f),
            new Vector3(-0.433013f, -0.250000f, -0.104167f),
            new Vector3(-0.249999f, -0.433013f, -0.083333f),
            new Vector3(0.000001f, -0.500000f, -0.062500f),
            new Vector3(0.250001f, -0.433012f, -0.041667f),
            new Vector3(0.433013f, -0.249999f, -0.020833f),
            new Vector3(0.500000f, 0.000001f, 0.000000f),
            new Vector3(0.433012f, 0.250001f, 0.020833f),
            new Vector3(0.250000f, 0.433013f, 0.041667f),
            new Vector3(0.000000f, 0.500000f, 0.062500f),
            new Vector3(-0.250000f, 0.433013f, 0.083333f),
            new Vector3(-0.433013f, 0.250000f, 0.104167f),
            new Vector3(-0.500000f, 0.000001f, 0.125000f),
            new Vector3(-0.433013f, -0.249999f, 0.145833f),
            new Vector3(-0.250001f, -0.433012f, 0.166667f),
            new Vector3(-0.000001f, -0.500000f, 0.187500f),
            new Vector3(0.249999f, -0.433014f, 0.208333f),
            new Vector3(0.433012f, -0.250001f, 0.229167f),
            new Vector3(0.500000f, -0.000002f, 0.250000f),
            new Vector3(0.433014f, 0.249998f, 0.270833f),
            new Vector3(0.250003f, 0.433011f, 0.291666f),
            new Vector3(0.000003f, 0.500000f, 0.312500f),
            new Vector3(-0.249997f, 0.433014f, 0.333333f),
            new Vector3(-0.433011f, 0.250003f, 0.354166f),
            new Vector3(-0.500000f, 0.000003f, 0.375000f),
            new Vector3(-0.433015f, -0.249997f, 0.395833f),
            new Vector3(-0.250004f, -0.433011f, 0.416666f),
            new Vector3(-0.000004f, -0.500000f, 0.437500f),
            new Vector3(0.249996f, -0.433015f, 0.458333f),
            new Vector3(0.433010f, -0.250004f, 0.479166f),
            new Vector3(0.500000f, -0.000004f, 0.500000f),
        };
    }
}