using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BaseballParser
{
    /// <summary>
    /// Dictionary of string-&gt;TValue which supports searching in the dictionary without needing to allocate a string object
    /// </summary>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    class IdDictionary<TValue>
    {
        ReaderWriterLock _valueCreationLock = new ReaderWriterLock();
        Dictionary<Substring, TValue> _dict = new Dictionary<Substring, TValue>();

        public IdDictionary()
        {
        }

        public TValue this[in Substring key]
        {
            get
            {
                TValue value;
                if (!TryGet(key, out value))
                {
                    throw new KeyNotFoundException();
                }

                return value;
            }
        }

        public bool TryGet(in Substring key, out TValue value)
        {
            _valueCreationLock.AcquireReaderLock(-1);
            try
            {
                if (_dict.TryGetValue(key, out value))
                {
                    return true;
                }
            }
            finally
            {
                _valueCreationLock.ReleaseReaderLock();
            }

            return false;
        }

        public TValue GetOrAdd(in Substring key, Func<string, object, TValue> valueFactory, object context)
        {
            TValue result;

            if (TryGet(key, out result))
            {
                return result;
            }

            _valueCreationLock.AcquireWriterLock(-1);
            try
            {
                // check again now that we have the write lock
                if (_dict.TryGetValue(key, out result))
                {
                    return result;
                }

                Substring keyToAdd = key.ShrinkBuffer();
                result = valueFactory(keyToAdd.Buffer, context);

                _dict.Add(keyToAdd, result);

                return result;
            }
            finally
            {
                _valueCreationLock.ReleaseWriterLock();
            }
        }

        // NOTE: This method should only be used after all the values are created as it has no locking
        public IReadOnlyCollection<TValue> GetValues()
        {
            return _dict.Values;
        }
    }
}
