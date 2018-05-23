using System;
using System.Collections.Generic;

namespace Augment
{
    /// <summary>
    /// A non-thread safe collection that provides lookups for both a primary key
    /// and unique key (obviously lending terminology from your DB knowledge)
    /// </summary>
    public abstract class MultiKeyCollection<TItem, TPrimaryKey, TUniqueKey> : SingleKeyCollection<TItem, TPrimaryKey>
    {
        #region Members

        private Dictionary<TUniqueKey, TItem> _byUniqueKey = new Dictionary<TUniqueKey, TItem>();

        #endregion

        #region Constructors

        public MultiKeyCollection()
            : base()
        {
            _byUniqueKey = new Dictionary<TUniqueKey, TItem>();
        }

        public MultiKeyCollection(IEqualityComparer<TUniqueKey> uniqueKeyComparer)
            : base()
        {
            _byUniqueKey = new Dictionary<TUniqueKey, TItem>(uniqueKeyComparer);
        }

        public MultiKeyCollection(IEqualityComparer<TPrimaryKey> primaryKeyComparer, IEqualityComparer<TUniqueKey> uniqueKeyComparer)
            : base(primaryKeyComparer)
        {
            _byUniqueKey = new Dictionary<TUniqueKey, TItem>(uniqueKeyComparer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TUniqueKey GetUniqueKey(TItem item);

        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();

            _byUniqueKey.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);

            UpdateUniqueKey(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);

            UpdateUniqueKey(item);
        }

        private void UpdateUniqueKey(TItem item)
        {
            TUniqueKey uq = GetUniqueKey(item);

            if (_byUniqueKey.ContainsKey(uq))
            {
                string msg = "Item already exists for Unique Key '{0}' on '{1}'".FormatArgs(uq, typeof(TItem).Name);

                throw new InvalidOperationException(msg);
            }

            _byUniqueKey[uq] = item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            TItem item = this[index];

            base.RemoveItem(index);

            TUniqueKey uq = GetUniqueKey(item);

            if (_byUniqueKey.ContainsKey(uq))
            {
                _byUniqueKey.Remove(uq);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uq"></param>
        /// <returns></returns>
        public bool ContainsUniqueKey(TUniqueKey uq)
        {
            return _byUniqueKey.ContainsKey(uq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uq"></param>
        /// <returns></returns>
        public TItem GetByUniqueKey(TUniqueKey uq)
        {
            return _byUniqueKey[uq];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uq"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetByUniqueKey(TUniqueKey uq, out TItem item)
        {
            return _byUniqueKey.TryGetValue(uq, out item);
        }

        #endregion
    }
}
