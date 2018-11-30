using System.Collections.Generic;
using EnsureThat;

namespace Augment
{
    /// <summary>
    /// Least Recently Used Cache
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    /// <remarks>
    /// Not an extension or a helper, but it's used in the datetime for storing off holidays
    /// </remarks>
    public class LeastRecentlyUsedCache<TKey, TItem> : IEnumerable<TItem> where TItem : class
    {
        #region Member Variables

        private class Entry
        {
            public TKey Key;
            public TItem Item;

            public Entry()
            {
            }

            public Entry(TKey key, TItem item)
            {
                Key = key;
                Item = item;
            }
        }

        private object _lock;
        private LinkedList<Entry> _linkedList;
        private Dictionary<TKey, LinkedListNode<Entry>> _entries;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="capacity"></param>
        public LeastRecentlyUsedCache(int capacity = 500)
        {
            _lock = new object();

            _linkedList = new LinkedList<Entry>();

            _entries = new Dictionary<TKey, LinkedListNode<Entry>>(capacity + 1);

            Capacity = capacity;

            Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears the cache of all objects
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _linkedList.Clear();

                _entries.Clear();
            }
        }

        /// <summary>
        /// Searches the cache for the key, if found the cache is NOT updated
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TItem FindByKey(TKey key)
        {
            lock (_lock)
            {
                if (_entries.ContainsKey(key))
                {
                    return _entries[key].Value.Item;
                }

                return null;
            }
        }

        /// <summary>
        /// Adds an item to the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TItem value)
        {
            lock (_lock)
            {
                Ensure.That(_entries.ContainsKey(key)).IsFalse();

                LinkedListNode<Entry> node = CreateLinkedListNode(key, value);

                Add(node);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        private void Add(LinkedListNode<Entry> node)
        {
            _entries.Add(node.Value.Key, node);

            _linkedList.AddFirst(node);

            ShrinkToCapacity();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            lock (_lock)
            {
                return _entries.ContainsKey(key);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(TKey key)
        {
            lock (_lock)
            {
                if (_entries.ContainsKey(key))
                {
                    LinkedListNode<Entry> node = _entries[key];

                    Remove(node);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        private void Remove(LinkedListNode<Entry> node)
        {
            _entries.Remove(node.Value.Key);

            _linkedList.Remove(node);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private LinkedListNode<Entry> CreateLinkedListNode(TKey key, TItem value)
        {
            Entry entry = new Entry(key, value);

            LinkedListNode<Entry> node = new LinkedListNode<Entry>(entry);

            return node;
        }

        /// <summary>
        ///
        /// </summary>
        private void ShrinkToCapacity()
        {
            while (EvictionNeeded)
            {
                LinkedListNode<Entry> node = _linkedList.Last;

                Remove(node);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TItem> GetItems()
        {
            foreach (Entry e in _linkedList)
            {
                yield return e.Item;
            }
        }

        #endregion

        #region Properities

        /// <summary>
        ///
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
            set
            {
                lock (_lock)
                {
                    _capacity = value;

                    ShrinkToCapacity();
                }
            }
        }

        private int _capacity;

        /// <summary>
        ///
        /// </summary>
        public TItem First
        {
            get
            {
                if (_linkedList.Count > 0)
                {
                    return _linkedList.First.Value.Item;
                }

                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public TItem Last
        {
            get
            {
                if (_linkedList.Count > 0)
                {
                    return _linkedList.Last.Value.Item;
                }

                return null;
            }
        }

        /// <summary>
        /// Remove from the cache?
        /// </summary>
        private bool EvictionNeeded
        {
            get { return Count > Capacity; }
        }

        /// <summary>
        ///
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get { return _entries.Keys; }
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<TItem> Items
        {
            get { return GetItems(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TItem this[TKey key]
        {
            get
            {
                lock (_lock)
                {
                    LinkedListNode<Entry> node = _entries[key];

                    _linkedList.Remove(node);

                    _linkedList.AddFirst(node);

                    return node.Value.Item;
                }
            }
            set
            {
                lock (_lock)
                {
                    if (_entries.ContainsKey(key))
                    {
                        _entries[key].Value.Item = value;
                    }
                    else
                    {
                        LinkedListNode<Entry> node = CreateLinkedListNode(key, value);

                        Add(node);
                    }
                }
            }
        }

        #endregion

        #region IEnumerable Implementation

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        #endregion
    }
}