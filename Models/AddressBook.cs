using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.net.Models
{
    [Table("address_books")]
    public class AddressBook
    {
        [Required]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        [Required]
        [Column("customer_id")]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        [Column ("name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Column("phone")]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [Column("address_line")]
        [StringLength(255)]
        public string AddressLine { get; set; }

        [Required]
        [Column("ward")]
        public int Ward {  get; set; }

        [Required]
        [Column("district")]
        public int District { get; set; }

        [Required]
        [Column("Province")]
        public int Province { get; set; }

        [Column("create_at")]
        public DateTime? CreateAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
