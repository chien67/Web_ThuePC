using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class OrderListRow
    {
        public int OrderId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string DeviceRequirement { get; set; }
        public int Quantity { get; set; }
        public int RentDays { get; set; }
        public decimal UnitPrice { get; set; }
        public byte Status { get; set; }

        public byte CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string RepresentativeName { get; set; }

        public string DisplayCustomerName =>
            !string.IsNullOrWhiteSpace(CustomerName) ? CustomerName : RepresentativeName;

        public string CustomerTypeText =>
            CustomerType == 1 ? "Doanh nghiệp" : "Cá nhân";
    }
}