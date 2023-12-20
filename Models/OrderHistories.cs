using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{
    public enum OrderHistoriesState
    {
        [EnumMember(Value = "draft")]
        Draft,
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "confirmed")]
        Confirmed,
        [EnumMember(Value = "delivering")]
        Delivering,
        [EnumMember(Value = "delivered")]
        Delivered,
        [EnumMember(Value = "cancelled")]
        Cancelled,
        [EnumMember(Value = "refunded")]
        Refunded,
        [EnumMember(Value = "failed")]
        Failed
    }
    [Table ("order_histories")]
    public class OrderHistories
    {
        [Required]
        [Column("order_id")]
        [Key]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(OrderHistoriesState))]
        public string State { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public Order Order { get; set; }

    }
}
