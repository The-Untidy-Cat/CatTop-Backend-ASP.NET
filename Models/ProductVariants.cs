using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
namespace asp.net.Models
{
    enum VariantState
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

        [Column("description")]
        [Display(Name = "description")]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [Column("product_id")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [Column("standard_price")]
        [Display(Name = "standard_price")]
        public long StandardPrice { get; set; }

        [Column("tax_rate")]
        [Display(Name = "tax_rate")]
        public double TaxRate { get; set; }

        [Column("discount")]
        [Display(Name = "discount")]
        public double Discount { get; set; }

        [Column("extra_fee")]
        [Display(Name = "extra_fee")]
        public double ExtraFee { get; set; }

        [Column("cost_price")]
        [Display(Name = "cost_price")]
        public long CostPrice { get; set; }

        [Column("sale_price")]
        [Display(Name = "sale_price")]
        public long SalePrice { get; set; }

        [Column("specifications")]
        [Display(Name = "specifications")]
        [StringLength(255)]
        public string Specifications { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(VariantState))]
        public string State { get; set; }

        [Column("created_at")]
        [Display(Name = "created_at")]
        public DateTime Created_at { get; set; }

        [Column("updated_at")]
        [Display(Name = "updated_at")]
        public DateTime Updated_at { get; set; }

        public Product Product { get; set; }

        public Cart Cart { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }

    }
}