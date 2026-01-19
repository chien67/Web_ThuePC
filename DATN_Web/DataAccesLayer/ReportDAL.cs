using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DATN_Web.DataAccesLayer
{
    public class ReportDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }

        // Doanh thu theo tháng
        public Dictionary<int, decimal> GetRevenueByMonth(int year)
        {
            var result = new Dictionary<int, decimal>();

            string sql = @"
            SELECT MONTH(PaidDate) AS Thang, SUM(TotalAmount) AS DoanhThu
            FROM Bills
            WHERE YEAR(PaidDate) = @Year
            GROUP BY MONTH(PaidDate)";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Year", year);
                conn.Open();

                var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    result.Add(
                        Convert.ToInt32(rd["Thang"]),
                        Convert.ToDecimal(rd["DoanhThu"])
                    );
                }
            }
            return result;
        }

        // Khách hàng mới theo tháng
        public Dictionary<int, int> GetNewCustomersByMonth(int year)
        {
            var result = new Dictionary<int, int>();

            string sql = @"
            SELECT MONTH(CreatedDate) AS Thang, COUNT(*) AS SoKhach
            FROM Customers
            WHERE YEAR(CreatedDate) = @Year
            GROUP BY MONTH(CreatedDate)";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Year", year);
                conn.Open();

                var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    result.Add(
                        Convert.ToInt32(rd["Thang"]),
                        Convert.ToInt32(rd["SoKhach"])
                    );
                }
            }
            return result;
        }
    }
}