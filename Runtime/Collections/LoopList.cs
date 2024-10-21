using System;
using System.Collections;
using System.Collections.Generic;
using ED.Additional.Utilities;

namespace ED.Additional.Collections
{
    public class LoopList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly List<T> _list;
        private int _startIndex = default;
        private int _version = default;

        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public IReadOnlyList<T> RawList => _list;

        public int StartIndex
        {
            get => _startIndex;
            set
            {
                value = ValidateIndex(value);
                if (value != _startIndex)
                {
                    _startIndex = value;
                    _version++;
                }
            }
        }

        public LoopList()
        {
            _list = new List<T>();
        }

        public LoopList(IEnumerable<T> items, int startIndex = 0)
        {
            _list = new List<T>(items);
            _startIndex = ValidateIndex(startIndex);
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            _list.Insert(_startIndex, item);
            _startIndex = ValidateIndex(_startIndex + 1);
            _version++;
        }

        public void Clear()
        {
            _list.Clear();
            _startIndex = 0;
            _version++;
        }

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(_startIndex, array, arrayIndex, Count - _startIndex);
            _list.CopyTo(0, array, arrayIndex + Count - _startIndex, _startIndex);
        }

        public bool Remove(T item)
        {
            var index = _list.IndexOf(item);
            if (index < 0) return false;
            _list.RemoveAt(index);
            if (index < _startIndex) _startIndex--;
            _version++;
            return true;
        }

        public int IndexOf(T item)
        {
            var result = _list.IndexOf(item);
            if (result < 0) return result;
            return ValidateIndex(result + _startIndex);
        }

        public void Insert(int index, T item)
        {
            index = ValidateIndex(index + _startIndex);
            _list.Insert(index, item);
            if (index < _startIndex) _startIndex++;
            _version++;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            if (index < _startIndex) _startIndex--;
            _version++;
        }

        public T this[int index]
        {
            get => _list[ValidateIndex(index + _startIndex)];
            set
            {
                _list[ValidateIndex(index + _startIndex)] = value;
                _version++;
            }
        }

        private int ValidateIndex(int index) => MathUtility.Loop(index, 0, Count);

        public struct Enumerator : IEnumerator<T>
        {
            private readonly LoopList<T> _list;
            private readonly int _version;
            private int _index;

            public T Current => _list[_index];
            object IEnumerator.Current => Current;

            public Enumerator(LoopList<T> list)
            {
                _list = list;
                _version = list._version;
                _index = -1;
            }

            public void Reset()
            {
                CheckVersion();
                _index = -1;
            }

            public bool MoveNext()
            {
                CheckVersion();
                _index++;
                return _index < _list.Count;
            }

            private void CheckVersion()
            {
                if (_list._version != _version)
                    throw new InvalidOperationException("Collection was modified!");
            }

            public void Dispose() { }
        }
    }
}