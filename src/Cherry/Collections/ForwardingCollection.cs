using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cherry.Collections
{
    /// <summary>
    /// This class provides a wrapper around a given collection object.
    /// It is mainly provided to simplify extending collection classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ForwardingCollection<T, TColl>
        : ICollection<T>, ICollection
        where TColl : ICollection<T>, ICollection
    {
        private readonly TColl _backingCollection;
        private readonly ICollection _backingCollectionNonGeneric;

        public ForwardingCollection(TColl backingCollection)
        {
            _backingCollection = backingCollection;
            _backingCollectionNonGeneric = _backingCollection;
        }

        protected TColl BackingCollection => _backingCollection;

        public virtual int Count => _backingCollectionNonGeneric.Count;

        public virtual bool IsSynchronized
        {
            get
            {
                CheckNonGenericImplemented();
                Debug.Assert(_backingCollectionNonGeneric != null);
                return _backingCollectionNonGeneric.IsSynchronized;
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                CheckNonGenericImplemented();
                Debug.Assert(_backingCollectionNonGeneric != null);
                return _backingCollectionNonGeneric.SyncRoot;
            }
        }

        public virtual bool IsReadOnly => _backingCollection.IsReadOnly;

        public virtual void Add(T item) => _backingCollection.Add(item);

        public virtual void Clear() => _backingCollection.Clear();

        public virtual bool Contains(T item) =>
            _backingCollection.Contains(item);

        public virtual void CopyTo(Array array, int index)
        {
            CheckNonGenericImplemented();
            Debug.Assert(_backingCollectionNonGeneric != null);
            _backingCollectionNonGeneric.CopyTo(array, index);
        }

        public virtual void CopyTo(T[] array, int arrayIndex) =>
            _backingCollection.CopyTo(array, arrayIndex);

        public virtual IEnumerator GetEnumerator()
        {
            CheckNonGenericImplemented();
            Debug.Assert(_backingCollectionNonGeneric != null);
            return _backingCollectionNonGeneric.GetEnumerator();
        }

        public virtual bool Remove(T item) => _backingCollection.Remove(item);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            _backingCollection.GetEnumerator();

        private void CheckNonGenericImplemented()
        {
            if (_backingCollectionNonGeneric is null)
            {
                throw new InvalidOperationException(
                    "The underlying collection must implement the System.Collections.ICollection interface.");
            }
        }
    }
}
