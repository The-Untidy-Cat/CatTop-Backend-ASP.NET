using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    enum ProductState
    {
        [EnumMember(Value = "draft")]
        Draft,
        [EnumMember(Value = "published")]
        Published,
        [EnumMember(Value = "archived")]
        Archived,
        [EnumMember(Value = "out_of_stock")]
        Out_of_stock
    }
    [Table("products")]
    public class Product
    {
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(ProductState))]
        [StringLength(255)]
        public string State { get; set; }

        [Required]
        [Column("slug")]
        public string Slug { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("created_at")]
        public DateTime? Created_at { get; set; }

        [Column("updated_at")]
        public DateTime? Updated_at { get; set; }

        [Column("brand_id")]
        [ForeignKey("Brand")]
        public int Brand_id { get; set; }
        public Brand Brand { get; set; }
        public virtual ICollection<ProductVariants> ProductVariants { get; set; }

    }
}
