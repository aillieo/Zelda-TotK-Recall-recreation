// -----------------------------------------------------------------------
// <copyright file="FixedQueue`1.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;

    using System.Collections;
    using System.Collections.Generic;

    public class FixedQueue<T> : IEnumerable<T>, ICollection<T>
    {
        private readonly T[] data;
        private int head;
        private int tail;
        private int count;

        public FixedQueue(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("Size must be greater than 0.", nameof(size));
            }

            this.data = new T[size];
            this.head = 0;
            this.tail = 0;
            this.count = 0;
        }

        public int Count => this.count;

        public bool IsReadOnly => false;

        public void Enqueue(T item)
        {
            if (this.count == this.data.Length)
            {
                // Remove the first item to make room for the new item.
                this.Dequeue();
            }

            this.data[this.tail] = item;
            this.tail = (this.tail + 1) % this.data.Length;
            this.count++;
        }

        public T Dequeue()
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            T item = this.data[this.head];
            this.data[this.head] = default;
            this.head = (this.head + 1) % this.data.Length;
            this.count--;

            return item;
        }

        public T Peek()
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            return this.data[this.head];
        }

        public bool TryGetItem(int index, out T item)
        {
            if (index >= 0 && index < this.count)
            {
                item = this.data[(this.head + index) % this.data.Length];
                return true;
            }
            else
            {
                item = default;
                return false;
            }
        }

        public void Add(T item)
        {
            this.Enqueue(item);
        }

        public void Clear()
        {
            this.head = 0;
            this.tail = 0;
            this.count = 0;

            for (var i = 0; i < this.data.Length; i++)
            {
                this.data[i] = default;
            }
        }

        public bool Contains(T item)
        {
            var index = Array.IndexOf(this.data, item, this.head, this.count);
            return index != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length || array.Length - arrayIndex < this.count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (this.count > 0)
            {
                if (this.head + this.count <= this.data.Length)
                {
                    Array.Copy(this.data, this.head, array, arrayIndex, this.count);
                }
                else
                {
                    var firstPart = this.data.Length - this.head;
                    Array.Copy(this.data, this.head, array, arrayIndex, firstPart);
                    Array.Copy(this.data, 0, array, arrayIndex + firstPart, this.count - firstPart);
                }
            }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException("FixedSizeQueue does not support removing an arbitrary item.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < this.count; i++)
            {
                yield return this.data[(this.head + i) % this.data.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
