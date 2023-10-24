using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Katuusagi.MemoizationForUnity
{
    public class LockFreeCacheValue<T> : IDictionary
    {
        private static readonly int ProcessorCount = Environment.ProcessorCount;

        private class CacheContainer
        {
            public CacheContainer Next;
            public T Result;
            public bool IsCached;
        }

        private class Pool
        {
            private CacheContainer _head = null;

            public void Return(CacheContainer newNode)
            {
                newNode.Result = default;
                newNode.IsCached = false;

                do
                {
                    newNode.Next = _head;
                }
                while (!CAS(ref _head, newNode.Next, newNode));
            }

            public bool TryGet(out CacheContainer node)
            {
                do
                {
                    node = _head;
                    if (node == null)
                    {
                        node = null;
                        return false;
                    }
                }
                while (!CAS(ref _head, node, node.Next));

                node.Next = null;
                return true;
            }

            private bool CAS(ref CacheContainer location, CacheContainer comparand, CacheContainer newValue)
            {
                return comparand == Interlocked.CompareExchange(ref location, newValue, comparand);
            }
        }

        private CacheContainer _current = new CacheContainer();
        private Pool _containerPool = new Pool();

        public T Result
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _current.Result;
        }

        public bool IsCached
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _current.IsCached;
        }

        public LockFreeCacheValue()
        {
            for (int i = 0; i < ProcessorCount; ++i)
            {
                _containerPool.Return(new CacheContainer());
            }
        }

        public void Set(in T value)
        {
            if (!_containerPool.TryGet(out var container))
            {
                var wait = new SpinWait();
                do
                {
                    wait.SpinOnce();
                }
                while (!_containerPool.TryGet(out container));
            }

            container.Result = value;
            container.IsCached = true;
            container = Interlocked.Exchange(ref _current, container);
            _containerPool.Return(container);
        }

        public void Clear()
        {
            if (!_containerPool.TryGet(out var container))
            {
                var wait = new SpinWait();
                do
                {
                    wait.SpinOnce();
                }
                while (!_containerPool.TryGet(out container));
            }

            container = Interlocked.Exchange(ref _current, container);
            _containerPool.Return(container);
        }

        bool IDictionary.IsFixedSize => throw new NotImplementedException();

        bool IDictionary.IsReadOnly => throw new NotImplementedException();

        ICollection IDictionary.Keys => throw new NotImplementedException();

        ICollection IDictionary.Values => throw new NotImplementedException();

        int ICollection.Count => throw new NotImplementedException();

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        object IDictionary.this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        void IDictionary.Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        bool IDictionary.Contains(object key)
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
