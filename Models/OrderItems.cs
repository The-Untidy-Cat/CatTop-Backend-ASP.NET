﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    public enum Rating
    {
        [EnumMember(Value = "1")]
        One,
        [EnumMember(Value = "2")]
        Two,
        [EnumMember(Value = "3")]
        Three,
        [EnumMember(Value = "4")]
        Four,
        [EnumMember(Value = "5")]
        Five
    }


    [Table ("order_items")]    
    public class OrderItems
    {
        [Required]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("order_id")]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [Column("variant_id")]
        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

        [Required]
        [Column("amount")]
        public int Amount { get; set; }

        [Required]
        [Column("standard_price")]
        public double StandardPrice { get; set; }

        [Required]
        [Column("sale_price")]
        public double SalePrice { get; set; }

        [Required]
        [Column("total")]
        public double Total { get; set; }

        [Required]
        [Column("is_refunded")]
        public bool IsRefunded { get; set; }

        [Column("rating")]
        [EnumDataType(typeof(Rating))]
        public string Rating { get; set; }

        [Column("review")]
        public string Review { get; set; }

        [Column("serial_number")]
        public string SerialNumber { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
