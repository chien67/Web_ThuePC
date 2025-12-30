using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeviceRequirement { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int RentDays { get; set; }
        public int Quantity { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal UnitPrice{ get; set; }
        public decimal DepositAmount { get; set; }
        public string Note { get; set; }
        public byte Status { get; set; }
    }
}