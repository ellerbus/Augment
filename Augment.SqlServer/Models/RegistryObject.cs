using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Augment.SqlServer.Models
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class RegistryObject
    {
        #region Constructors

        public RegistryObject(string registryName, string sqlScript, string statusEnum)
        {
            _registryName = registryName;
            _sqlScript = sqlScript;
            _statusEnum = statusEnum;
        }

        public RegistryObject(SqlObject sqlObj)
        {
            RegistryName = sqlObj.NormalizedName;
            SqlScript = sqlObj.OriginalSql;
            Status = Status.Updated;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return RegistryName + (IsModified ? " *" : "");
        }

        public string ToMergeSql()
        {
            StringBuilder sql = new StringBuilder();

            string table = $"merge dbo.AugmentRegistry as tgt";

            string select = $"using (select '{GetSafeString(RegistryName)}' registry_name, '{GetSafeString(SqlScript)}' sql_script, '{GetSafeString(UpdatedBy)}' updated_by, '{StatusEnum}' status_enum) as src";

            string on = "on (tgt.registry_name = src.registry_name)";

            string update = $"when matched then update set tgt.sql_script = src.sql_script, tgt.updated_utc = getutcdate(), tgt.updated_by = src.updated_by, tgt.status_enum = src.status_enum";

            string insert = $"when not matched by target then insert (registry_name, sql_script, status_enum, updated_utc, updated_by) values (src.registry_name, src.sql_script, 'Created', getutcdate(), src.updated_by)";

            return $"{table} {select} {on} {update} {insert} ;";
        }

        private static string GetSafeString(string text)
        {
            return text.Replace("'", "''");
        }

        private void UpdateModifiedFlag<T>(ref T source, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(source, value))
            {
                IsModified = true;

                source = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string RegistryName
        {
            get { return _registryName; }
            private set { UpdateModifiedFlag(ref _registryName, value); }
        }
        private string _registryName;

        /// <summary>
        /// 
        /// </summary>
        public string SqlScript
        {
            get { return _sqlScript; }
            set { UpdateModifiedFlag(ref _sqlScript, value); }
        }
        private string _sqlScript;

        /// <summary>
        /// 
        /// </summary>
        public string StatusEnum
        {
            get { return _statusEnum; }
            private set { UpdateModifiedFlag(ref _statusEnum, value); }
        }
        private string _statusEnum;

        /// <summary>
        /// 
        /// </summary>
        public Status Status
        {
            get
            {
                return StatusEnum.AssertNotNull("None").ToEnum<Status>();
            }
            set { StatusEnum = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedUtc
        {
            get { return DateTime.UtcNow; }
            private set { UpdateModifiedFlag(ref _updatedUtc, value); }
        }
        private DateTime _updatedUtc;

        /// <summary>
        /// 
        /// </summary>
        public string UpdatedBy
        {
            get { return Environment.UserName; }
            private set { UpdateModifiedFlag(ref _updatedBy, value); }
        }
        private string _updatedBy;

        /// <summary>
        /// 
        /// </summary>
        public bool IsModified { get; private set; }

        #endregion
    }
}
