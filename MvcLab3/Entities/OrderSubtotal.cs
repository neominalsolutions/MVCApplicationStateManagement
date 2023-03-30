using System;
using System.Collections.Generic;

namespace MvcLab3.Entities
{
    public partial class OrderSubtotal
    {
        public int OrderId { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
