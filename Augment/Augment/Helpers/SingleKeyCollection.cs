using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Augment
{
    /// <summary>
    /// A non-thread safe collection that provides lookups for both a primary key
    /// and unique key (obviously lending terminology from your DB knowledge)
    /// </summary>
    public abstract class SingleKeyCollection<TItem, TPrimaryKey> : Collection<TItem>
    {
        #region Members

        private Dictionary<TPrimaryKey, TItem> _byPrimaryKey = new Dictionary<TPrimaryKey, TItem>();

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TPrimaryKey GetPrimaryKey(TItem item);

        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();

            _byPrimaryKey.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);

            UpdatePrimaryKey(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);

            UpdatePrimaryKey(item);
        }

        private void UpdatePrimaryKey(TItem item)
        {
            TPrimaryKey pk = GetPrimaryKey(item);

            if (_byPrimaryKey.ContainsKey(pk))
            {
                string msg = "Item already exists for Primary Key '{0}' on '{1}'".FormatArgs(pk, typeof(TItem).Name);

                throw new InvalidOperationException(msg);
            }

            _byPrimaryKey[pk] = item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            TItem item = this[index];

            base.RemoveItem(index);

            TPrimaryKey pk = GetPrimaryKey(item);

            if (_byPrimaryKey.ContainsKey(pk))
            {
                _byPrimaryKey.Remove(pk);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        public bool ContainsPrimaryKey(TPrimaryKey pk)
        {
            return _byPrimaryKey.ContainsKey(pk);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        public TItem GetByPrimaryKey(TPrimaryKey pk)
        {
            return _byPrimaryKey[pk];
        }

        #endregion
    }
}
