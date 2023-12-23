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
        [EnumMember(Value = "customer")]
        Customer,
        [EnumMember(Value = "seller")]
        Seller
    }
    [Table("user_roles")]
    public class UserRole
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("role_id")]
        [EnumDataType(typeof(UserRoleEnum))]
        public string RoleId { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
