using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string DeviceRequirement { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}