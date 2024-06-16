using System;
using System.Collections.Generic;

namespace ED.Additional.Collections
{
    public class KeySelectionSortedList<TKey, TValue> : List<TValue> where TKey : IComparable
    {
        private readonly Comparer comparer;

        public KeySelectionSortedList(Func<TValue, TKey> keySelector) : base() {
            comparer = new Comparer(keySelector);
        }

        public new void Add(TValue item) {
            base.Add(item);
            base.Sort(comparer);
        }

        public new void AddRange(IEnumerable<TValue> collection) {
            base.AddRange(collection);
            base.Sort(comparer);
        }
            
        public class Comparer : IComparer<TValue>
        {
            private readonly Func<TValue, TKey> keySelector;
            private readonly Comparer<TKey> comparer;
            public Comparer(Func<TValue, TKey> keySelector) {
                this.keySelector = keySelector;
                comparer = Comparer<TKey>.Default;
            }
            public int Compare(TValue x, TValue y) {
                return comparer.Compare(keySelector(x), keySelector(y));
            }
        }
    }
}