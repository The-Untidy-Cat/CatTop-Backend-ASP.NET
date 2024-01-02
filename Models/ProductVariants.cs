using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    public class Cpu
    {
        public string? Name { get; set; }
        public int? Cores { get; set; }
        public int? Threads { get; set; }
        public int? BaseClock { get; set; }
        public int? TurboClock { get; set; }
        public int? Cache { get; set; }

        public override string ToString()
        {
            return $"{Name}, {BaseClock}-{TurboClock}GHz";
        }
    }

    public class Ram
    {
        public int? Capacity { get; set; }
        public string? Type { get; set; }
        public int? Frequency { get; set; }

        public override string ToString()
        {
            return $"{Capacity}GB {Type} {Frequency}MHz";
        }
    }

    public class Storage
    {
        public string? Drive { get; set; }
        public string? Capacity { get; set; }
        public string? Type { get; set; }
        public override string ToString()
        {
            return $"{Capacity}GB {Drive} {Type}";
        }
    }

    public class Display
    {
        public string? Size { get; set; }
        public string? Resolution { get; set; }
        public string? Technology { get; set; }
        public int? RefreshRate { get; set; }
        public bool? Touch { get; set; }
        public override string ToString()
        {
            return $"{Size}, {Resolution}, {Technology}, {Touch}";
        }
    }

    public class Gpu
    {
        public string? Name { get; set; }
        public int? Memory { get; set; }
        public string? Type { get; set; }
        public int? Frequency { get; set; }
        public override string ToString()
        {
            return $"{Name} {Memory}GB {Type} {Frequency}MHz";
        }
    }

    public class Specifications
    {
        public Cpu? Cpu { get; set; }
        public Ram? Ram { get; set; }
        public Storage? Storage { get; set; }
        public Display? Display { get; set; }
        public Gpu? Gpu { get; set; }
        public string? Ports { get; set; }
        public string? Keyboard { get; set; }
        public bool? Touchpad { get; set; }
        public bool? Webcam { get; set; }
        public int? Battery { get; set; }
        public double? Weight { get; set; }
        public string? Os { get; set; }
        public int? Warranty { get; set; }
        public string? Color { get; set; }

    }

    public class SpecificationSummary
    {
        public string Color { get; set; }
        public string Cpu { get; set; }
        public string Ram { get; set; }
        public string Storage { get; set; }
        public string Display { get; set; }
        public string Card { get; set; }

        // Constructor for deserialization
        public SpecificationSummary()
        {
        }

        // Custom method to create a SpecificationSummary object from a Laptop object
        public static SpecificationSummary FromSpecifications(Specifications laptop)
        {
            return new SpecificationSummary
            {
                Color = laptop?.Color,
                Cpu = $"{laptop?.Cpu?.Name} {laptop?.Cpu?.BaseClock}GHz",
                Ram = $"{laptop?.Ram?.Capacity}GB {laptop?.Ram?.Type}",
                Storage = $"{laptop?.Storage?.Capacity}GB {laptop?.Storage?.Drive}",
                Display = $"{laptop?.Display?.Size}, {laptop?.Display?.Resolution}, {laptop?.Display?.Technology}, {laptop?.Display?.Touch}",
                Card = $"{laptop?.Gpu?.Name} {laptop?.Gpu?.Type}"
            };
        }
    }

    [Table("product_variants")]
    public class ProductVariants
    {
        [Required]
        [Column("id")]
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("sku")]
        [Display(Name = "sku")]
        [StringLength(255)]
        public string? SKU { get; set; }

        [Required]
        [Column("name")]
        [Display(Name = "name")]
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(VariantState))]
        [StringLength(255)]
        public string? State { get; set; }

        [Required]
        [Column("product_id")]
        public long ProductID { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("image")]
        [Url]
        public string? Image { get; set; }

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

        [Column("specifications", TypeName = "json")]
        public string? Specifications { get; set; }

        [Column("created_at")]
        public DateTime? Created_at { get; set; }

        [Column("updated_at")]
        public DateTime? Updated_at { get; set; }

        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
        public ICollection<OrderItems>? OrderItems { get; set; }

    }
}
