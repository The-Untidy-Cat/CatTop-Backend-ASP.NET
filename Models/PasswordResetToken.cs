using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace asp.net.Models
{
    [Table("password_reset_tokens")]
    public class PasswordResetToken
    {
        [Key]
        public string Email { get; set; }
        public string Token { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
