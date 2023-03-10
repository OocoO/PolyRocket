using System;
using System.Collections;
using System.Collections.Generic;

namespace Carotaa.Code
{
    public class CycleArray<T> : IEnumerable<T>
    {
        public readonly int Size;
        private readonly T[] _buffer;
        private readonly int _bufferLen;

        private int _head;
        private int _tail;

        public CycleArray(int size)
        {
            _bufferLen = size + 1; // there is always a empty node
            _buffer = new T[_bufferLen];

            _head = 0;
            _tail = 0;
            Size = size;
        }

        // the not empty node of this Cycle Array
        public int Count => (_tail - _head + _bufferLen) % _bufferLen;

        /// <summary>
        ///     i could be (-Count, Count), -2 mean the second one start form head
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i]
        {
            get
            {
                if (i >= 0 && i < Count)
                    return _buffer[GetIndex(_tail - i - 1)];
                if (i < 0 && i >= -Count) return _buffer[GetIndex(_head + i)];

                throw new IndexOutOfRangeException();
            }
        }

        // return if cycle array is full
        public bool IsFull => GetIndex(_tail + 1) == _head;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _head = _tail;
        }


        public void Add(T item)
        {
            _buffer[_tail] = item;

            _tail = GetIndex(_tail + 1);

            // if is overwrite a head node. head go next
            if (_tail >= _head) _head = GetIndex(_head + 1);
        }

        public T Pop()
        {
            if (Count <= 0)
                throw new Exception("Cycle Array is Empty");

            var temp = _buffer[_head];
            _head = GetIndex(_head + 1);

            return temp;
        }

        private int GetIndex(int ind)
        {
            return (ind + _bufferLen) % _bufferLen;
        }
    }
}