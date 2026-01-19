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
        public decimal DepositAmount { get; set; }

        // 🔹 Dữ liệu thô (từ SQL / BLL)
        public byte CustomerType { get; set; }          // 1 = Cá nhân, 2 = Doanh nghiệp
        public string CustomerName { get; set; }        // Cá nhân: tên | DN: tên công ty
        public string RepresentativeName { get; set; } // DN: người đại diện
        // Tên hiển thị trên UI
        public string DisplayCustomerName
        {
            get
            {
                if (CustomerType == 1) // Cá nhân
                    return CustomerName;

                if (CustomerType == 2) // Doanh nghiệp
                {
                    if (!string.IsNullOrWhiteSpace(RepresentativeName))
                        return $"{CustomerName} ({RepresentativeName})";

                    return CustomerName;
                }

                return CustomerName;
            }
        }

        // Text loại khách
        public string CustomerTypeText =>
            CustomerType == 1 ? "Cá nhân" : "Doanh nghiệp";
    }
}