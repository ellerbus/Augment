using System.Collections;
using System.Collections.Generic;
using EnsureThat;

namespace Augment.SqlServer.Development.Models
{
    public class RegistryObjectCollection : IEnumerable<RegistryObject>
    {
        #region Methods

        public override string ToString()
        {
            return $"Count={Count}";
        }

        public void Add(SqlObject sqlObj)
        {
            switch (sqlObj.Type)
            {
                case ObjectTypes.SystemScript:
                    break;

                default:
                    RegistryObject regObj = null;

                    if (Dictionary.TryGetValue(sqlObj.NormalizedName, out regObj))
                    {
                        //  get the script updated
                        regObj.SqlScript = sqlObj.OriginalSql;
                    }
                    else
                    {
                        regObj = new RegistryObject(sqlObj);

                        Add(regObj);
                    }

                    regObj.Status = Status.Updated;
                    break;
            }
        }

        public void Drop(SqlObject sqlObj)
        {
            RegistryObject regObj = null;

            if (Dictionary.TryGetValue(sqlObj.NormalizedName, out regObj))
            {
                regObj.Status = Status.Deleted;
            }
        }

        public void Add(RegistryObject regObj)
        {
            Ensure.That(Contains(regObj))
                .WithExtraMessageOf(() => $"'{regObj.ToString()}' Already in Collection")
                .IsFalse();

            Dictionary.Add(regObj.RegistryName, regObj);
        }

        public bool Contains(RegistryObject regObj)
        {
            return Dictionary.ContainsKey(regObj.RegistryName);
        }

        public bool Contains(SqlObject sqlObj)
        {
            return Dictionary.ContainsKey(sqlObj.NormalizedName);
        }

        public RegistryObject Find(RegistryObject regObj)
        {
            RegistryObject found = null;

            Dictionary.TryGetValue(regObj.RegistryName, out found);

            return found;
        }

        public IEnumerator<RegistryObject> GetEnumerator()
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

        private IDictionary<string, RegistryObject> Dictionary { get; } = new Dictionary<string, RegistryObject>();

        #endregion
    }
}
