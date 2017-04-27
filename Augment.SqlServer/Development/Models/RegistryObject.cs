using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Augment.SqlServer.Mapping;
using Augment.SqlServer.Properties;

namespace Augment.SqlServer.Development.Models
{
    [DebuggerDisplay("{RegistryName,nq}"), Table("AugmentRegistry", Schema = "dbo")]
    public class RegistryObject
    {
        #region Constructors

        public RegistryObject(string registryName, string sqlScript, string actionEnum, DateTime updatedUtc)
        {
            RegistryName = registryName;
            SqlScript = sqlScript;
            ActionEnum = actionEnum;
            UpdatedUtc = updatedUtc;
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
            return $"{RegistryName}";
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

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [Key, Required, Column("registry_name")]
        public string RegistryName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, Column("sql_script")]
        public string SqlScript { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [Required, Column("action_enum")]
        public string ActionEnum { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public Action Action
        {
            get { return ActionEnum.AssertNotNull("None").ToEnum<Action>(); }
            set { ActionEnum = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required, Column("updated_utc")]
        public DateTime UpdatedUtc { get; private set; }

        #endregion
    }
}
