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
        public bool IsPaid { get; set; }

        //  Dữ liệu thô (từ SQL / BLL)
        public byte CustomerType { get; set; }          // 1 = Cá nhân, 2 = Doanh nghiệp
        public string CustomerName { get; set; }        // DN: tên công ty (Cá nhân thường null)
        public string RepresentativeName { get; set; } // Cá nhân: họ tên | DN: người đại diện
        public string DisplayCustomerName
        {
            get
            {
                // Cá nhân: hiển thị họ tên (RepresentativeName)
                if (CustomerType == 1)
                    return RepresentativeName;

                // Doanh nghiệp: hiển thị "Tên công ty (Người đại diện)" nếu có
                if (CustomerType == 2)
                {
                    if (!string.IsNullOrWhiteSpace(RepresentativeName))
                        return $"{CustomerName} ({RepresentativeName})";

                    return CustomerName;
                }

                // fallback
                return !string.IsNullOrWhiteSpace(CustomerName) ? CustomerName : RepresentativeName;
            }
        }

        // Text loại khách
        public string CustomerTypeText =>
            CustomerType == 1 ? "Cá nhân" : "Doanh nghiệp";
    }
}