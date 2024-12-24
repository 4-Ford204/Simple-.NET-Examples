using System;

namespace Example.Learning
{
    public class Indexer<T>
    {
        private T[] Array { get; set; } = new T[100];

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Array.Length)
                    throw new IndexOutOfRangeException("Index Out Of Range Exception");
                else
                    return Array[index];
            }

            set
            {
                if (index < 0 || index >= Array.Length)
                    throw new IndexOutOfRangeException("Index Out Of Range Exception");
                else
                    Array[index] = value;
            }
        }
    }
}
