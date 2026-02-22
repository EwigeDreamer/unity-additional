using System;
using System.Collections.Generic;

namespace ED.Additional.Utilities
{
    public static class ArrayPool<T>
    {
        private static readonly Dictionary<int, List<T[]>> Pools = new();
        
        public static T[] Get(int length)
        {
            if (!Pools.TryGetValue(length, out var pool) || pool.Count == 0)
                return new T[length];
            var array = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            return array;
        }

        public static PooledObject Get(int length, out T[] array)
        {
            array = Get(length);
            return new ArrayPool<T>.PooledObject { Array = array };
        }

        public static void Release(T[] array)
        {
            if (array == null) return;
            for (int i = 0; i < array.Length; i++) array[i] = default;
            if (Pools.TryGetValue(array.Length, out var pool)) pool.Add(array);
            else Pools[array.Length] = new List<T[]> { array };
        }

        public struct PooledObject : IDisposable
        {
            public T[] Array;
            public void Dispose() => Release(this.Array);
        }
    }
}