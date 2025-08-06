using System;
using UnityEditor;
using UnityEngine;

namespace ED.Additional.Editor
{
    public static class HandlesUtility
    {
        public static ColorHandler BeginColor(Color color) => new(color);
        public static MatrixHandler BeginMatrix(Matrix4x4 matrix) => new(matrix);
        
        public struct ColorHandler : IDisposable
        {
            private readonly Color _colorTmp;

            public ColorHandler(Color color)
            {
                _colorTmp = Handles.color;
                Handles.color = color;
            }

            public void Dispose()
            {
                Handles.color = _colorTmp;
            }
        }
        
        public struct MatrixHandler : IDisposable
        {
            private readonly Matrix4x4 _matrixTmp;

            public MatrixHandler(Matrix4x4 matrix)
            {
                _matrixTmp = Handles.matrix;
                Handles.matrix = matrix;
            }

            public void Dispose()
            {
                Handles.matrix = _matrixTmp;
            }
        }
    }
}