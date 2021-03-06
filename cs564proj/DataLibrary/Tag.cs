using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;

namespace DataLibrary
{
    public class Tag
    {
        [Name("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.None)] public int TagId { get; set; }
        [Name("value")] public string Name { get; set; }
        [Ignore]
        public List<UserTag> UserTag { get; set; }
    }
}