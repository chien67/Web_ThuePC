using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class DashboardChartVM
    {
        public List<string> Labels { get; set; }          // Tháng
        public List<decimal> RevenueData { get; set; }    // Doanh thu
        public List<int> CustomerData { get; set; }       // Khách mới
    }
}