using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    public enum UserRoleEnum
    {
        [EnumMember(Value = "admin")]
        Admin,
        [EnumMember(Value = "user")]
        User,
        [EnumMember(Value = "seller")]
        Seller
    }
    [Table("user_roles")]
    public class UserRole
    {
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Column("role_id")]
        [EnumDataType(typeof(UserRoleEnum))]
        public int RoleId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
