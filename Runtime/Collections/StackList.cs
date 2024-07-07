using System;
using System.Collections.Generic;

namespace ED.Additional.Collections
{
    public class StackList<T> : List<T>
    {
        public void Push(T item) => Add(item);

        public T Peek()
        {
            if (Count > 0) return this[Count - 1];
            
            throw new InvalidOperationException($"{nameof(StackList<T>)}<{typeof(T).Name}> is empty");
        }

        public T Pop()
        {
            if (Count > 0)
            {
                var result = Peek();
                RemoveAt(Count - 1);
                return result;
            }
            
            throw new InvalidOperationException($"{nameof(StackList<T>)}<{typeof(T).Name}> is empty");
        }

        public bool TryPeek(out T item)
        {
            item = default;
            if (Count == 0)
                return false;
            item = Peek();
            return true;
        }

        public bool TryPop(out T item)
        {
            item = default;
            if (Count == 0)
                return false;
            item = Pop();
            return true;
        }
    }
}