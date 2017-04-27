using System.Collections;
using System.Collections.Generic;
using EnsureThat;

namespace Augment.SqlServer.Development.Models
{
    public class SqlObjectCollection : IEnumerable<SqlObject>
    {
        #region Methods

        public override string ToString()
        {
            return $"Count={Count}";
        }

        public void Add(SqlObject sqlObj)
        {
            Ensure.That(Contains(sqlObj))
                .WithExtraMessageOf(() => $"'{sqlObj.ToString()}' Already in Collection")
                .IsFalse();

            Dictionary.Add(sqlObj.NormalizedName, sqlObj);
        }

        public bool Contains(SqlObject sqlObj)
        {
            return Dictionary.ContainsKey(sqlObj.NormalizedName);
        }

        public SqlObject Find(SqlObject sqlObj)
        {
            SqlObject found = null;

            Dictionary.TryGetValue(sqlObj.NormalizedName, out found);

            return found;
        }

        public SqlObject Find(RegistryObject regObj)
        {
            SqlObject found = null;

            Dictionary.TryGetValue(regObj.RegistryName, out found);

            return found;
        }

        public IEnumerator<SqlObject> GetEnumerator()
        {
            return Dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.Values.GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get { return Dictionary.Count; } }

        private IDictionary<string, SqlObject> Dictionary { get; } = new Dictionary<string, SqlObject>();

        #endregion
    }
}
