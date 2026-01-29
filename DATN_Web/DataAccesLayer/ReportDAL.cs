using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.DataAccesLayer
{
    public class ReportDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }

        public ReportFilterVM GetRevenueAndNewCustomers(DateTime from, DateTime toInclusive)
        {
            string sql = @"
                SELECT
                    -- Doanh thu
                    ISNULL((
                        SELECT SUM(TotalAmount)
                        FROM Bills
                        WHERE Status = 1
                          AND PaidDate >= @From AND PaidDate <= @To
                    ), 0) AS Revenue,

                    -- Khách hàng mới
                    ISNULL((
                        SELECT COUNT(*)
                        FROM Customers
                        WHERE CreatedDate >= @From AND CreatedDate <= @To
                    ), 0) AS NewCustomers;";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = from;
                cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = toInclusive;

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new ReportFilterVM
                        {
                            Revenue = Convert.ToDecimal(rd["Revenue"]),
                            NewCustomers = Convert.ToInt32(rd["NewCustomers"])
                        };
                    }
                }
            }

            return new ReportFilterVM();
        }
    }
}