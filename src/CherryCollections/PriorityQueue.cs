using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SE = CherryCollections.StandardExceptionMessages;

namespace CherryCollections
{
    /// <summary>
    /// A PriorityQueue&lt;T&gt; is a collection of items of type T. The
    /// largest item is always at the head of the queue and can be peeked
    /// or dequeued from the queue.
    /// </summary>
    /// <remarks>
    /// <para>There are three ways in which items can be prioritised.
    /// <list type="bullet">
    ///     <item>T implements <see cref="IComparable{T}" /> or
    ///     <see cref="IComparable"/>, then the CompareTo method of that
    ///     interface will be used to compare items unless one of the other
    ///     two ways is used.</item>
    ///     <item>Alternatively, a custom comparision function
    ///     can be passed in as a delegate or as an implementation of
    ///     <see cref="IComparer{T}"/>. These two ways can be used to
    ///     come up with more complex strategies for prioritisation
    ///     including the trivial case of a min-priority queue where
    ///     the comparision can simply order the elements in the reverse
    ///     order.</item>
    /// </list>
    /// </para>
    /// <para>Updates to items' properties which in turn affect how they are
    /// compared by any of the above ways will render all priority guarentees
    /// made by this queue invalid. Instead use the <see cref="UpdatePriority"/>
    /// method.</para>
    /// <para>null values can be stored in this queue provided that the chosen
    /// comparision method supports null values as well.</para>
    /// <para>This class is not thread safe.</para>
    /// <para>This class is serializable if and only if the type
    /// <typeparamref name="T"/>is also serializable.</para>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [Serializable]
    public sealed class PriorityQueue<T> :
        ICollection<T>, ICollection, ICloneable
    {

        #region Public API

        /// <summary>
        /// Creates an empty priority queue to store items of type T. Items
        /// will be prioritised using the default comparator for type T.
        /// T must implement <see cref="IComparable{T}" />.
        /// </summary>
        public PriorityQueue()
            : this(Array.Empty<T>()) { }

        /// <summary>
        /// Creates an empty priority queue to store items of type T. Items
        /// will be prioritised using the given comparision function.
        /// T must implement <see cref="IComparable{T}" />.
        /// </summary>
        /// <param name="comparer">The function which will</param>
        public PriorityQueue(Comparison<T> comparer)
            : this(Array.Empty<T>(), Comparer<T>.Create(comparer)) { }

        /// <summary>
        /// Creates an empty priority queue to store items of type T. Items
        /// will be prioritised using the given comparator.
        /// T must implement <see cref="IComparable{T}" />.
        /// </summary>
        public PriorityQueue(IComparer<T> comparer)
            : this(Array.Empty<T>(), comparer) { }

        /// <summary>
        /// Copies the elements from the given enumerable and builds a priority
        /// queue from them using the default comparator for the elements
        /// in the enumerable. The items must implement
        /// <see cref="IComparable{T}" />.
        /// </summary>
        /// <param name="from">An enumerable of comparable objects.</param>
        /// <param name="comparison">A delegate to a function that should order
        /// items with highest priority as greater than items with lower
        /// priorities.</param>
        public PriorityQueue(IEnumerable<T> from)
            : this(from, Comparer<T>.Default) { }

        /// <summary>
        /// Copies the elements from the given enumerable and builds a priority
        /// queue from them using the given comparision delegate for the
        /// elements in the enumerable.
        /// </summary>
        /// <param name="from">An enumerable of comparable objects.</param>
        /// <param name="comparison">A delegate to a function that should order
        /// items with highest priority as greater than items with lower
        /// priorities.</param>
        public PriorityQueue(IEnumerable<T> from, Comparison<T> comparison)
            : this(from, Comparer<T>.Create(comparison)) { }

        /// <summary>
        /// Copies the elements from the given enumerable and builds a heap
        /// out of them using the given comparer.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="comparer">The comparer that should order
        /// items with highest priority as greater than items with lower
        /// priorities.</param>
        public PriorityQueue(IEnumerable<T> from, IComparer<T> comparer)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            bool heapify = true;
            if (from is PriorityQueue<T> another)
            {
                _backingArray = new T[another.Count];
                if (another.Count > 0)
                {
                    Array.Copy(another._backingArray,
                        _backingArray, another.Count);
                }
                Count = another.Count;
                heapify = !another._comparer.Equals(comparer);
            }
            else
            {
                _backingArray = from.ToArray();
                Count = _backingArray.Length;
            }

            _comparer = comparer;
            if (heapify)
            {
                BuildMaxHeap();
            }
        }

        /// <summary>
        /// Extracts the highest priority element out of the queue failing
        /// with an exception when there are no elements left in the queue.
        /// </summary>
        /// <returns>The highest priority element.</returns>
        /// <exception cref="InvalidOperationException">If this queue
        /// has no elements left.</exception>
        public T Dequeue()
        {
            return ExtractMax();
        }

        /// <summary>
        /// Returns the element with the highest priority in this queue without
        /// dequeuing it.
        /// </summary>
        /// <returns>The highest priority.</returns>
        /// <exception cref="InvalidOperationException">If there are
        /// no elements in this queue.</exception>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException(SE.NoElemsInCollection);
            }
            else
            {
                return _backingArray[0];
            }
        }

        /// <summary>
        /// Iterates this queue with no guarentees on ordering. 
        /// This method is provided to allow you to copy the contents of this
        /// queue to other collections. Modifications to this queue are not
        /// permitted while the queue is being iterated over.
        /// </summary>
        /// <returns>An enumerator that iterates on this queue.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _backingArray.Take(Count).GetEnumerator();
        }

        /// <summary>
        /// Iterates this queue with no guarentees on ordering. 
        /// This method is provided to allow you to copy the contents of this
        /// queue to other collections. Modifications to this queue are not
        /// permitted while the queue is being iterated over.
        /// </summary>
        /// <returns>An enumerator that iterates on this queue.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enqueues a new item into this queue.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Add(T item) => Enqueue(item);

        /// <summary>
        /// Enqueues all given items into this queue.
        /// </summary>
        /// <param name="items">The items to enqueue.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items is ICollection<T> coll)
            {
                GrowIfNeccessary(Count + coll.Count);
                coll.CopyTo(_backingArray, Count);
                Count += coll.Count;
            }
            else
            {
                foreach (var item in items)
                {
                    GrowIfNeccessary(Count + 1);
                    _backingArray[Count++] = item;
                }
            }
            BuildMaxHeap();
        }

        /// <summary>
        /// Enqueues a new item into this queue. Returns the
        /// head of the queue without dequeuing it.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue(T item)
        {
            MaxHeapInsert(item);
        }

        /// <summary>
        /// Clears this queue of all elements.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_backingArray, 0, _backingArray.Length);
            _backingArray = new T[MIN_GROW_SIZE];
            Count = 0;
        }

        /// <summary>
        /// True if and only if one or more items in this queue are
        /// equal (in the sense of <see cref="Object.Equals(object)"/>)
        /// to the given item.
        /// </summary>
        /// <remarks>This method in worst case will take linear time to 
        /// execute as it needs to check all objects in the queue. 
        /// If you need to call this operation frequently consider using an 
        /// <see cref="SortedSet{T}"/>.</remarks>
        /// <param name="item">The item to check.</param>
        /// <returns>True if this item is found, false otherwise.</returns>
        public bool Contains(T item)
        {
            var index = Array.IndexOf(_backingArray, item);
            return index >= 0 && index < Count;
        }

        /// <summary>
        /// Copies the elements of this queue to the given destination array
        /// starting at a given index in the destination array.
        /// </summary>
        /// <param name="array">The one dimensional array to which
        /// elements of this queue are to be copied.</param>
        /// <param name="arrayIndex">The index in the destination array at which
        /// copying is to begin.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException"> when the destination
        /// array does not have sufficient space after 
        /// <paramref name="arrayIndex"/> to store all elements of this queue.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"> when
        /// <paramref name="index"/> is less than 0</exception>.
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_backingArray, 0, array, arrayIndex, Count);
        }

        /// <summary>
        /// Removes an item from the priority queue which is equal to the given
        /// item in the sense of <see cref="Object.Equals(object?)"/>.
        /// If more than one items satisfy this condition then only one of
        /// them will be removed. No guarentees are made on which one
        /// would be removed.
        /// </summary>
        /// <remarks>This method in worst case will take linear time to 
        /// execute as it needs to search for the item in the queue
        /// before removing it. If you need to call this operation
        /// frequently consider using an <see cref="SortedSet{T}"/>.</remarks>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if an item was removed. False otherwise.</returns>
        public bool Remove(T item)
        {
            var index = FindIndex(item);
            if (index >= 0)
            {
                Delete(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the priority of the item in this queue which is equal to
        /// the given item in the sense of <see cref="Object.Equals(object?)"/>
        /// with the given item. If more than one items satisfy this condition
        /// then only one of them will be replaced.
        /// No guarentees are made on which one would be replaced.
        /// </summary>
        /// <remarks>This method in worst case will take linear time to 
        /// execute as it needs to search for the item in the queue
        /// before updating its priority. If you need to call this operation
        /// frequently consider using an <see cref="SortedSet{T}"/>.</remarks>
        /// <param name="item">The item whose priority is to be updated.</param>
        public void UpdatePriority(T item, Action<T> priorityUpdateFunction)
        {
            var index = FindIndex(item);
            if (index >= 0)
            {
                priorityUpdateFunction(_backingArray[index]);
                Rebalance(index);
            }
        }

        /// <summary>
        /// The number of items in this queue.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// This collection is not read only. This property will always
        /// return false.
        /// </summary>
        /// <returns>False.</returns>
        public bool IsReadOnly => false;

        /// <summary>
        /// Access to this collection is not thread safe. This property
        /// will always return false.
        /// </summary>
        /// <returns>False.</returns>
        public bool IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronise access to this
        /// queue.
        /// </summary>
        /// <returns>An object that can be used to synchronise access to this
        /// queue.</returns>
        public object SyncRoot => _syncRoot;

        /// <summary>
        /// Copies the contents of this priority queue to the given destination
        /// array starting at the given destination index. Contents are not
        /// copied in any specific order.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="index">The index in the destination array at which
        /// the copying should begin.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.RankException">
        /// <paramref name="array" /> is not of rank 1.</exception>
        /// <exception cref="T:System.ArrayTypeMismatchException">
        ///     <paramref name="array" /> is not of type
        ///     <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException"> when the destination
        /// array does not have sufficient space after <paramref name="index"/>
        /// to store all elements of this queue.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"> when
        /// <paramref name="index"/> is less than 0</exception>.
        public void CopyTo(Array array, int index)
        {
            Array.Copy(_backingArray, 0, array, index, Count);
        }

        /// <summary>
        /// Returns a shallow clone of this object. Changes made to this
        /// queue will not affect the copy. However, changes made to the items
        /// of the queue will affect the items stored in the copy if they have
        /// not been removed from the latter.
        /// </summary>
        /// <remarks>Special note: Calls to
        /// <see cref="UpdatePriority(T, Action{T})"/> need to be made on
        /// each queue which holds the item.</remarks>
        /// <returns>Another queue with all elements of this queue
        /// and which uses the same ordering way as this queue.</returns>
        public object Clone()
        {
            return new PriorityQueue<T>(this, _comparer);
        }

        #endregion

        #region Implementation

        private static readonly int GROW_FACTOR = 2;
        private static readonly int MIN_GROW_SIZE = 4;
        private static readonly double SHRINK_THRESHOLD_FACTOR = 0.9;

        private T[] _backingArray;
        private readonly IComparer<T> _comparer;
        private readonly object _syncRoot = new();

        private static int GetParent(int index) => (index - 1) / 2;

        private static int GetLeft(int index) => 2 * index + 1;

        private static int GetRight(int index) => 2 * index + 2;

        private int Compare(int index1, int index2)
        {
            if (index1 == index2)
            {
                return 0;
            }
            else
            {
                return _comparer.Compare(_backingArray[index1],
                    _backingArray[index2]);
            }
        }

        private int MaxIndex(int index1, params int[] indices)
        {
            int max = index1;
            foreach (var i in indices.Where(e => e < Count))
            {
                if (Compare(i, max) > 0)
                {
                    max = i;
                }
            }
            return max;
        }

        private void BuildMaxHeap()
        {
            for (var i = (Count - 1) / 2; i >= 0; i--)
            {
                MaxHeapify(i);
            }
        }

        private void MaxHeapify(int index)
        {
            while (true)
            {
                var left = GetLeft(index);
                var right = GetRight(index);
                int largest;
                if (left < Count && Compare(left, index) > 0)
                {
                    largest = left;
                }
                else
                {
                    largest = index;
                }

                if (right < Count && Compare(right, largest) > 0)
                {
                    largest = right;
                }

                if (largest != index)
                {
                    _backingArray.Swap(index, largest);
                    index = largest;
                    continue;
                }
                break;
            }
        }

        private T ExtractMax()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException(SE.NoElemsInCollection);
            }

            var max = _backingArray[0];
            _backingArray[0] = _backingArray[Count - 1];
            _backingArray[Count - 1] = default!;
            Count -= 1;
            ShrinkIfNeccessary();
            MaxHeapify(0);
            return max;
        }

        private void IncreaseKey(int index, T key)
        {
            _backingArray[index] = key;
            while (index > 0 && Compare(GetParent(index), index) < 0)
            {
                _backingArray.Swap(index, GetParent(index));
                index = GetParent(index);
            }
        }

        private void MaxHeapInsert(T key)
        {
            Count += 1;
            GrowIfNeccessary(Count);
            IncreaseKey(Count - 1, key);
        }

        private void Delete(int index)
        {
            if (Compare(index, Count - 1) < 0)
            {
                IncreaseKey(index, _backingArray[Count - 1]);
                Count -= 1;
            }
            else
            {
                _backingArray[index] = _backingArray[Count - 1];
                Count -= 1;
                MaxHeapify(0);
            }
        }

        private int FindIndex(T item)
        {
            var index = Array.IndexOf(_backingArray, item);
            if (index >= Count)
            {
                return -1;
            }
            return index;
        }

        private void Rebalance(int index)
        {
            if (Compare(index, GetParent(index)) > 0 && index != 0)
            {
                BubbleUp(index);
            }
            else
            {
                BubbleDown(index);
            }
        }

        private void BubbleDown(int index)
        {
            var left = GetLeft(index);
            var right = left + 1;
            int maxIndex = MaxIndex(index, left, right);
            while (index != maxIndex && left < Count && right < Count)
            {
                _backingArray.Swap(index, maxIndex);

                index = maxIndex;
                left = GetLeft(index);
                right = left + 1;
                maxIndex = MaxIndex(index, left, right);
            }
        }

        private void BubbleUp(int index)
        {
            var parent = GetParent(index);
            while (Compare(parent, index) < 0 && index != 0)
            {
                _backingArray.Swap(parent, index);

                index = GetParent(index);
                parent = GetParent(index);
            }
        }

        private void ShrinkIfNeccessary()
        {
            if (_backingArray.Length >= SHRINK_THRESHOLD_FACTOR * Count)
            {
                Array.Resize(ref _backingArray, Count);
            }
        }

        private void GrowIfNeccessary(int targetSize)
        {
            if (_backingArray.Length <= targetSize)
            {
                int newCapacity = GROW_FACTOR * _backingArray.Length;
                if ((uint)newCapacity > int.MaxValue)
                {
                    newCapacity = int.MaxValue;
                }
                newCapacity = Math.Max(newCapacity,
                    _backingArray.Length + MIN_GROW_SIZE);
                Array.Resize(ref _backingArray, newCapacity);
            }
        }

        #endregion

    }
}