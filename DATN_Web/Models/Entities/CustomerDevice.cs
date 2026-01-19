using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.Models.Enum;

namespace DATN_Web.Models.Entities
{
    public class CustomerDevice
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ModelId { get; set; }
        public int? DeliveryUserId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int Quantity { get; set; }
        public CustomerDeviceStatus Status { get; set; } = CustomerDeviceStatus.WaitingReceive;
        public DateTime? ReturnDate { get; set; }
    }
}