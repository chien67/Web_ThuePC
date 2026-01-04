using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class CustomerDevice
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ModelId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int Quantity { get; set; }
        public byte Status { get; set; } = 1;
        public DateTime? ReturnDate { get; set; }
    }
}