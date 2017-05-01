using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Dapper;

namespace Augment.SqlServer.Development.Models
{
    [DebuggerDisplay("{ToString(),nq}"), Table("AugmentRegistry", Schema = "dbo")]
    public class RegistryObject
    {
        #region Constructors

        [ExplicitConstructor]
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
        [Required, Column("status_enum")]
        public string StatusEnum
        {
            get { return _statusEnum; }
            private set { UpdateModifiedFlag(ref _statusEnum, value); }
        }
        private string _statusEnum;

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
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
        [Required, Column("updated_utc")]
        public DateTime UpdatedUtc
        {
            get { return DateTime.UtcNow; }
            private set { UpdateModifiedFlag(ref _updatedUtc, value); }
        }
        private DateTime _updatedUtc;

        /// <summary>
        /// 
        /// </summary>
        [Required, Column("updated_by")]
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
