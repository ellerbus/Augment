using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Augment.Tests.SqlServer.Mapping
{
    [Table("Person", Schema = "dbo")]
    class Person
    {
        [Column("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name"), Required]
        public string Name { get; set; }

        [Column("rowversion"), Timestamp]
        public byte[] RowVersion { get; set; }

        public bool HasChildren { get; set; }

        [NotMapped]
        public DateTime DateOfBirth { get; set; }
    }
}