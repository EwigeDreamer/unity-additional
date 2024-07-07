using System;
using System.Collections;
using System.Collections.Generic;

namespace ED.Additional.Collections
{
    public class HashList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly List<T> _list;
        private readonly HashSet<T> _set;

        public HashList()
        {
            _list = new List<T>();
            _set = new HashSet<T>();
        }

        public HashList(int capacity)
        {
            _list = new List<T>(capacity);
            _set = new HashSet<T>(capacity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(T item)
        {
            if (!_set.Add(item)) return;
            _list.Add(item);
        }

        public void Clear()
        {
            _set.Clear();
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            arrayIndex = Math.Max(arrayIndex, 0);
            var count = Math.Min(array.Length, _list.Count);
            for (var i = arrayIndex; i < count; ++i) array[i] = _list[i];
        }

        public bool Remove(T item)
        {
            _set.Remove(item);
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _list.Count) throw new ArgumentOutOfRangeException(nameof(index));
            if (_set.Contains(item)) return;
            _set.Add(item);
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _list.Count) throw new ArgumentOutOfRangeException(nameof(index));
            _set.Remove(_list[index]);
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _list.Count) throw new ArgumentOutOfRangeException(nameof(index));
                return _list[index];
            }
            set
            {
                if (index < 0 || index >= _list.Count) throw new ArgumentOutOfRangeException(nameof(index));
                if (_set.Contains(value)) return;
                _set.Remove(_list[index]);
                _set.Add(value);
                _list[index] = value;
            }
        }
    }
}