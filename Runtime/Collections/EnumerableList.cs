using System.Collections;
using System.Collections.Generic;

namespace ED.Additional.Collections
{
    public class EnumerableList<T> : List<T>, IEnumerator<T>
    {
        public EnumerableList(IEnumerable<T> collection) : base(collection) { }
        public int CurrentIndex { get; private set; } = -1;
        public bool IsFirst => CurrentIndex == 0;
        public bool IsLast => CurrentIndex == Count - 1 && CurrentIndex >= 0;
        public bool MoveNext() => ++CurrentIndex < Count;
        object IEnumerator.Current => Current;
        public T Current => this[CurrentIndex];
        
        public void Reset() => CurrentIndex = -1;
        public void Dispose() => Reset();
    }

    public static class EnumerableListExtensions
    {
        public static EnumerableList<T> ToEnumerableList<T>(this IEnumerable<T> source) => new EnumerableList<T>(source);
    }
}