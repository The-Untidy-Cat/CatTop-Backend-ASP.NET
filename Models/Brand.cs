using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    enum BrandState
    {
        [EnumMember(Value = "draft")]
        Draft,
        [EnumMember(Value = "active")]
        Active,
        [EnumMember(Value = "inactive")]
        Inactive
    }
    [Table("brands")]
    public class Brand
    {
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [Display(Name = "name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Column("slug")]
        [Display(Name = "slug")]
        [StringLength(255)]
        public string Slug { get; set; }

        [Column("description")]
        [Display(Name = "description")]
        [StringLength(255)]
        public string Description { get; set; }

        [Column("created_at")]
        [Display(Name = "created_at")]
        [Timestamp]
        public DateTime? Created_at { get; set; }

        [Column("updated_at")]
        [Display(Name = "updated_at")]
        [Timestamp]
        public DateTime? Updated_at { get; set; }

        [Required]
        [Column("state")]
        [Display(Name = "state")]
        [StringLength(255)]
        public string State { get; set; }

    }
}
