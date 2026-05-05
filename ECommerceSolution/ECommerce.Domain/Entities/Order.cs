using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        // Foreign key
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        public string BillingFirstName { get; set; } = string.Empty;
        public string BillingLastName { get; set; } = string.Empty;
        public string BillingEmail { get; set; } = string.Empty;
        public string BillingPhoneNumber { get; set; } = string.Empty;
        public string BillingStreet { get; set; } = string.Empty;
        public string BillingBuilding { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingCountry { get; set; } = "EG";
        public string BillingState { get; set; } = "Cairo";
        public string BillingApartment { get; set; } = "NA";
        public string BillingFloor { get; set; } = "NA";

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }
}
