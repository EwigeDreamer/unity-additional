using System;
using System.Collections.Generic;

namespace EwigeDreamer.Additional.Collections
{
    public class FixedSizeQueue<T> : Queue<T>
    {
        public readonly int Size;

        public FixedSizeQueue(int size) : base(size + 1)
        {
            if (size < 1) throw new ArgumentException("Must be greater than 0", nameof(size));
            Size = size;
        }

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            while (Count > Size) Dequeue();
        }
    }
}