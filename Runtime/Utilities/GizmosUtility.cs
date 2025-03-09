using System;
using UnityEngine;

namespace ED.Additional.Utilities
{
    public static class GizmosUtility
    {
        public static void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool crosshair = false)
        {
            using var h1 = BeginLocation(position, rotation, scale * 0.5f);
            using var h2 = BeginColor(color);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2f);
            if (crosshair)
            {
                Gizmos.DrawLine(_pt1, _pb3);
                Gizmos.DrawLine(_pt2, _pb4);
                Gizmos.DrawLine(_pt3, _pb1);
                Gizmos.DrawLine(_pt4, _pb2);
            }
        }

        private static readonly Vector3 _pt1 = new(1f, 1f, 1f);
        private static readonly Vector3 _pt2 = new(-1f, 1f, 1f);
        private static readonly Vector3 _pt3 = new(-1f, 1f, -1f);
        private static readonly Vector3 _pt4 = new(1f, 1f, -1f);
        private static readonly Vector3 _pb1 = new(1f, -1f, 1f);
        private static readonly Vector3 _pb2 = new(-1f, -1f, 1f);
        private static readonly Vector3 _pb3 = new(-1f, -1f, -1f);
        private static readonly Vector3 _pb4 = new(1f, -1f, -1f);
        
        public static void DrawWireSphere(Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool crosshair = false)
        {
            using var h1 = BeginLocation(position, rotation, scale * 0.5f);
            using var h2 = BeginColor(color);
            Gizmos.DrawWireSphere(Vector3.zero, 1f);
            if (crosshair)
            {
                Gizmos.DrawLine(Vector3.left, Vector3.right);
                Gizmos.DrawLine(Vector3.forward, Vector3.back);
                Gizmos.DrawLine(Vector3.up, Vector3.down);
            }
        }
        
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
    }
}