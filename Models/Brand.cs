﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string? Name { get; set; }

        [Column("slug")]
        [StringLength(255)]
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [Column("description")]
        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [Column("state")]
        [StringLength(255)]
        [EnumDataType(typeof(BrandState))]
        public string? State { get; set; }

        [Column("image")]
        [StringLength(255)]
        [Url]
        public string? Image { get; set; }

        [Column("parent_id")]
        [ForeignKey("brandPa")]
        public int? ParentId { get; set; }
        public Brand? brandPa { get; set; }
        public Brand? brandMain { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
