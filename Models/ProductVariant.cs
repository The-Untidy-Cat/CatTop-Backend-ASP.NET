using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.net.Models
{
    [Table ("product_variants")]
    public class ProductVariant
    {
        [Required]
        [Column("id")]
        [Key]
        public int Id { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }

    
}
