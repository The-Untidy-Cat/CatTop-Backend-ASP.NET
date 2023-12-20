using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    public enum CustomerState
    {
        [EnumMember(Value = "draft")]
        Draft,
        [EnumMember(Value = "active")]
        Active,
        [EnumMember(Value = "inactive")]
        Inactive,
        [EnumMember(Value = "banned")]
        Banned
    }

    [Table("customers")]
    public class Customer
    {
        [Required]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("first_name")]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [Column("last_name")]
        [StringLength(255)]
        public string LastName { get; set; }

        [Required]
        [Column("email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column("phone_number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        public int? gender { get; set; }

        [Column("state")]
        [EnumDataType(typeof(CustomerState))]
        public string State { get; set; }

        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        public User User { get; set; }

        [Column("created_at")]
        [Display(Name = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        [Display(Name = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public virtual AddressBook AddressBook { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
