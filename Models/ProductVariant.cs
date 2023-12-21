using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
namespace asp.net.Models
{
    public enum VariantState
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
    [Table("product_variants")]
    public class ProductVariants
    {
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("sku")]
        [Display(Name = "sku")]
        [StringLength(255)]
        public string SKU { get; set; }

        [Required]
        [Column("name")]
        [Display(Name = "name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(VariantState))]
        [StringLength(255)]
        public string State { get; set; }

        [Required]
        [Column("product_id")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [Column("description")]
        public string Description { get; set; }



        [Column("standard_price")]
        public long StandardPrice { get; set; }

        [Column("tax_rate")]
        public double TaxRate { get; set; }

        [Column("discount")]
        public double Discount { get; set; }

        [Column("extra_fee")]
        public double ExtraFee { get; set; }

        [Column("cost_price")]
        public long CostPrice { get; set; }

        [Column("sale_price")]
        public long SalePrice { get; set; }

        [Column("specifications")]
        [StringLength(255)]
        public string Specifications { get; set; }



        [Column("created_at")]
        public DateTime? Created_at { get; set; }

        [Column("updated_at")]
        public DateTime? Updated_at { get; set; }

        public Product Product { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}
