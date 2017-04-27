using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Augment.SqlServer.Properties;

namespace Augment.SqlServer.Development.Models
{
    [DebuggerDisplay("{ToString(),nq}"), Table("AugmentRegistry", Schema = "dbo")]
    public class RegistryObject
    {
        #region Constructors

        public RegistryObject(string registryName, string sqlScript, string actionEnum, DateTime updatedUtc)
        {
            _registryName = registryName;
            _sqlScript = sqlScript;
            _actionEnum = actionEnum;
            _updatedUtc = updatedUtc;
        }

        public RegistryObject(SqlObject sqlObj)
        {
            RegistryName = sqlObj.NormalizedName;
            SqlScript = sqlObj.OriginalSql;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return RegistryName + (IsModified ? " *" : "");
        }

        public string ToMergeSql()
        {
            string sql = Resources.RegistryMergeScript
                .Replace("RegistryName", GetSafeString(RegistryName))
                .Replace("SqlScript", GetSafeString(SqlScript))
                .Replace("Action", GetSafeString(ActionEnum));

            return sql;
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
        [Key, Required, Column("registry_name")]
        public string RegistryName
        {
            get { return _registryName; }
            private set { UpdateModifiedFlag(ref _registryName, value); }
        }
        private string _registryName;

        /// <summary>
        /// 
        /// </summary>
        [Required, Column("sql_script")]
        public string SqlScript
        {
            get { return _sqlScript; }
            set { UpdateModifiedFlag(ref _sqlScript, value); }
        }
        private string _sqlScript;

        /// <summary>
        /// 
        /// </summary>
        [Required, Column("action_enum")]
        public string ActionEnum
        {
            get { return _actionEnum; }
            private set { UpdateModifiedFlag(ref _actionEnum, value); }
        }
        private string _actionEnum;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public Action Action
        {
            get
            {
                return ActionEnum.AssertNotNull("None").ToEnum<Action>();
            }
            set { ActionEnum = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required, Column("updated_utc")]
        public DateTime UpdatedUtc
        {
            get { return _updatedUtc; }
            private set { UpdateModifiedFlag(ref _updatedUtc, value); }
        }
        private DateTime _updatedUtc;

        /// <summary>
        /// 
        /// </summary>
        public bool IsModified { get; private set; }

        #endregion
    }
}
