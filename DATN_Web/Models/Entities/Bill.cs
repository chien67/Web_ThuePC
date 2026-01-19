using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class Bill
    {
        public int BillId { get; set; }
        public int OrderId { get; set; }

        public decimal RentalAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public byte CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string TaxCode { get; set; }
        public DateTime PaidDate { get; set; }
        public int PaidUserId { get; set; }

        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}