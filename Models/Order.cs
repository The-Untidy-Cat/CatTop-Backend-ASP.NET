using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace asp.net.Models
{

    public enum OrderState
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

    public enum PaymentMethod
    {
        [EnumMember(Value = "cash")]
        Cash,
        [EnumMember(Value = "banking")]
        Banking,
    }

    public enum ShoppingMethod
    {
        [EnumMember(Value = "online")]
        Online,
        [EnumMember(Value = "offline")]
        Offline,
    }

    public enum PaymentState
    {
        [EnumMember(Value = "unpaid")]
        Unpaid,
        [EnumMember(Value = "partially_paid")]
        PartiallyPaid,
        [EnumMember(Value = "paid")]
        Paid,
    }
    [Table("orders")]
    public class Order
    {
        [Required]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Column("employee_id")]
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        [Column("state")]
        [EnumDataType(typeof(OrderState))]
        public string State { get; set; }

        [Required]
        [Column("payment_method")]
        [EnumDataType(typeof(PaymentMethod))]
        public string PaymentMethod { get; set; }

        [Required]
        [Column("shopping_method")]
        [EnumDataType(typeof(ShoppingMethod))]
        public string ShoppingMethod { get; set; }

        [Required]
        [Column("payment_state")]
        [EnumDataType(typeof(PaymentState))]
        public string PaymentState { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("tracking_no")]
        public string? TrackingNo { get; set; }

        [Column("address_id")]
        public int? AddressId { get; set; }

        //public virtual decimal TotalPrice { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        
        public AddressBook? AddressBook { get; set; }
        public virtual ICollection<OrderHistories> OrderHistories { get; set; }
        public virtual ICollection<OrderItems>? OrderItems { get; set; }
    }
}


