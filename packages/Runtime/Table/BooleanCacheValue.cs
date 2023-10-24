using System;
using System.Collections;
using System.Collections.Generic;

namespace Katuusagi.MemoizationForUnity
{
    public class BooleanCacheValue<T> : IDictionary<bool, T>
    {
        private CacheValue<T> _trueValue = new CacheValue<T>();
        private CacheValue<T> _falseValue = new CacheValue<T>();

        public ICollection<bool> Keys => throw new NotImplementedException();

        public ICollection<T> Values => throw new NotImplementedException();

        public int Count
        {
            get
            {
                int count = 0;
                if (_trueValue.IsCached)
                {
                    ++count;
                }
                if (_falseValue.IsCached)
                {
                    ++count;
                }

                return count;
            }
        }

        public bool IsReadOnly => false;

        public T this[bool key]
        {
            get
            {
                if (key)
                {
                    if (_trueValue.IsCached)
                    {
                        return _trueValue.Result;
                    }
                }
                else
                {
                    if (_falseValue.IsCached)
                    {
                        return _falseValue.Result;
                    }
                }

                throw new KeyNotFoundException("The given key was not present in the dictionary.");
            }
            set
            {
                if (key)
                {
                    _trueValue.Set(value);
                }
                else
                {
                    _falseValue.Set(value);
                }
            }
        }

        public void Add(bool key, T value)
        {
            if (key)
            {
                if (_trueValue.IsCached)
                {
                    throw new ArgumentException("An item with the same key has already been added.");
                }
                _trueValue.Set(value);
            }
            else
            {
                if (_falseValue.IsCached)
                {
                    throw new ArgumentException("An item with the same key has already been added.");
                }
                _falseValue.Set(value);
            }
        }

        public bool ContainsKey(bool key)
        {
            if (key)
            {
                return _trueValue.IsCached;
            }
            else
            {
                return _falseValue.IsCached;
            }
        }

        public bool Remove(bool key)
        {
            if (key)
            {
                if (!_trueValue.IsCached)
                {
                    return false;
                }
                _trueValue.Clear();
                return true;
            }
            else
            {
                if (!_falseValue.IsCached)
                {
                    return false;
                }
                _falseValue.Clear();
                return true;
            }
        }

        public bool TryGetValue(bool key, out T value)
        {
            if (key)
            {
                if (!_trueValue.IsCached)
                {
                    value = default;
                    return false;
                }
                value = _trueValue.Result;
                return true;
            }
            else
            {
                if (!_falseValue.IsCached)
                {
                    value = default;
                    return false;
                }
                value = _falseValue.Result;
                return true;
            }
        }

        void ICollection<KeyValuePair<bool, T>>.Add(KeyValuePair<bool, T> item)
        {
            if (item.Key)
            {
                _trueValue.Set(item.Value);
            }
            else
            {
                _falseValue.Set(item.Value);
            }
        }

        bool ICollection<KeyValuePair<bool, T>>.Contains(KeyValuePair<bool, T> item)
        {
            if (item.Key)
            {
                if (!_trueValue.IsCached)
                {
                    return false;
                }
                return EqualityComparer<T>.Default.Equals(item.Value, _trueValue.Result);
            }
            else
            {
                if (!_falseValue.IsCached)
                {
                    return false;
                }
                return EqualityComparer<T>.Default.Equals(item.Value, _falseValue.Result);
            }
        }

        void ICollection<KeyValuePair<bool, T>>.CopyTo(KeyValuePair<bool, T>[] array, int arrayIndex)
        {
            int add = 0;
            if (_trueValue.IsCached)
            {
                array[arrayIndex + add] = new KeyValuePair<bool, T>(true, _trueValue.Result);
                ++add;
            }
            if (_falseValue.IsCached)
            {
                array[arrayIndex + add] = new KeyValuePair<bool, T>(true, _falseValue.Result);
            }
        }

        bool ICollection<KeyValuePair<bool, T>>.Remove(KeyValuePair<bool, T> item)
        {
            if (!((ICollection<KeyValuePair<bool, T>>)this).Contains(item))
            {
                return false;
            }

            if (item.Key)
            {
                _trueValue.Clear();
            }
            else
            {
                _falseValue.Clear();
            }

            return true;
        }

        public IEnumerator<KeyValuePair<bool, T>> GetEnumerator()
        {
            if (_trueValue.IsCached)
            {
                yield return new KeyValuePair<bool, T>(true, _trueValue.Result);
            }
            if (_falseValue.IsCached)
            {
                yield return new KeyValuePair<bool, T>(true, _falseValue.Result);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _trueValue.Clear();
            _falseValue.Clear();
        }
    }
}
