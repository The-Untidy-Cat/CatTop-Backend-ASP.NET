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
    }
}
