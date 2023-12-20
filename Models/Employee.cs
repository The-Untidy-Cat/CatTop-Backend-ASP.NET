using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    enum EmployeeState
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
    [Table("employees")]
    public class Employee
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("first_name")]
        [Display(Name = "first_name")]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Column("last_name")]
        [Display(Name = "last_name")]
        [StringLength(255)]
        public string LastName { get; set; }

        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
