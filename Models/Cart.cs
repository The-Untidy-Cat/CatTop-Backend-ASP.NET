using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.net.Models
{
    [Table("carts")]
    public class Cart
    {
        [Required]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("customer_id")]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [Required]
        [Column("variant_id")]
        [ForeignKey("Variant")]
        public long VariantId { get; set; }
        public ProductVariants Variant { get; set; }

        [Required]
        [Column("amount")]
        public int Amount { get; set; }

        [Column("create_at")]
        public DateTime? CreateAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
