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
        public long Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(ProductState))]
        [StringLength(255)]
        public string? State { get; set; }

        [Column("image")]
        [Url]
        public string? Image { get; set; }

        [Required]
        [Column("slug")]
        public string? Slug { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("brand_id")]
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        public ICollection<ProductVariants>? ProductVariants { get; set; }

        public long StandardPrice
        {
            get
            {
                long price = ProductVariants.Min(v => v.StandardPrice);
                return price;
            }
        }

        public long SalePrice
        {
            get
            {
                long price = ProductVariants.Min(v => v.SalePrice);
                return price;
            }
        }
    }
}
