using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class ReportFilterVM
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal Revenue { get; set; }
        public int NewCustomers { get; set; }
    }
}