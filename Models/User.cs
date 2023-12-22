using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    enum UserState
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

    [Table("users")]
    public class User
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [Column("password")]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(UserState))]
        public string State { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
    }
}
