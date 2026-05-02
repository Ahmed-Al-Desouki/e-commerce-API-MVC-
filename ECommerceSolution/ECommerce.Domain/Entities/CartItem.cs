using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        // Foreign keys
        public int CartId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigation properties
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
