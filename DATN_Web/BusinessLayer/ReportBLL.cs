using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.BusinessLayer
{
    public class ReportBLL
    {
        private readonly ReportDAL _dal = new ReportDAL();

        public DashboardChartVM GetDashboardChart(int year)
        {
            var revenue = _dal.GetRevenueByMonth(year);
            var customers = _dal.GetNewCustomersByMonth(year);

            var labels = new List<string>();
            var revenueData = new List<decimal>();
            var customerData = new List<int>();

            for (int m = 1; m <= 12; m++)
            {
                labels.Add($"Tháng {m}");
                revenueData.Add(revenue.ContainsKey(m) ? revenue[m] : 0);
                customerData.Add(customers.ContainsKey(m) ? customers[m] : 0);
            }

            return new DashboardChartVM
            {
                Labels = labels,
                RevenueData = revenueData,
                CustomerData = customerData
            };
        }
    }
}