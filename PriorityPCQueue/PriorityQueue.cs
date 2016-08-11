using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkoDevcic
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private const int DEFAULT_CAPACITY = 4;
        private T[] items;
        private int size = 0;

        private static T[] emptyArray = new T[0];

        public int Size { get { return size; } }

        public PriorityQueue()
        {
            items = emptyArray;
        }

        public PriorityQueue(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("Size must be 0 or greater");

            items = new T[size];
        }

        public PriorityQueue(IEnumerable<T> sourceCollection)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException("sourceCollection");

            var sourceSize = sourceCollection.Count();
            items = new T[sourceSize];
            for (int i = 0; i < sourceSize; i++)
            {
                InsertAndShift(sourceCollection.ElementAt(i));
            }
            size = sourceSize;
        }

        public void Clear()
        {
            items = emptyArray;
            size = 0;
        }

        public void Insert(T item)
        {
            InsertAndShift(item);
        }

        public void InsertRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            var requiredSize = size + collection.Count();
            if (requiredSize > items.Length)
                Enlarge(requiredSize);

            foreach (var item in collection)
            {
                InsertAndShift(item);
            }
        }

        private void InsertAndShift(T item)
        {
            if (size == items.Length)
                Enlarge();

            var index = size++;
            items[index] = item;
            ShiftUp(index);
        }

        private void ShiftUp(int index)
        {
            var parent = GetParent(index);
            while (index > 0 && items[index].CompareTo(items[parent]) >= 0)
            {
                Swap(index, parent);
                index = parent;
                parent = GetParent(index);
            }
        }

        private int GetParent(int index)
        {
            var parent = (index - 1) >> 1;
            return parent;
        }

        public T PeekTopItem()
        {
            if (size == 0)
                throw new InvalidOperationException("Empty queue");

            return items[0];
        }

        public T TakeTopItem()
        {
            var topItem = PeekTopItem();

            items[0] = items[--size];
            items[size] = default(T);

            ShiftDown();

            if (size <= items.Length >> 1)
                Reduce();

            return topItem;
        }

        public bool TryTakeTopItem(out T item)
        {
            item = default(T);
            if (size == 0)
            {
                return false;
            }
            else
            {
                item = TakeTopItem();
                return true;
            }
        }

        private void ShiftDown()
        {
            var root = 0;

            while (root * 2 + 1 < size)
            {
                int next;
                var left = root * 2 + 1;

                if (items[left].CompareTo(items[root]) > 0)
                    next = left;
                else
                    next = root;

                var right = root * 2 + 2;
                if (right < size && items[right].CompareTo(items[next]) > 0)
                    next = right;
                if (next != root)
                {
                    Swap(next, root);
                    root = next;
                }
                else
                    break;
            }
        }

        private void Reduce()
        {
            if (size >= DEFAULT_CAPACITY)
            {
                var newSize = items.Length >> 1;
                Resize(newSize, false);
            }
        }

        private void Enlarge()
        {
            var newSize = size == 0 ? DEFAULT_CAPACITY : size << 1;
            Resize(newSize, true);
        }

        private void Enlarge(int addToSize)
        {
            var newSize = size + addToSize;
            Resize(newSize, true);
        }

        private void Resize(int newSize, bool enlarge)
        {
            var newItems = new T[newSize];
            Array.Copy(items, 0, newItems, 0, enlarge ? size : newSize);
            items = newItems;
        }

        private void Swap(int first, int second)
        {
            var temp = items[first];
            items[first] = items[second];
            items[second] = temp;
        }
    }
}
