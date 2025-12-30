using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using DATN_Web.Models.Entities;

namespace DATN_Web.DataAccesLayer
{
    public class OrderDetailDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }

        public bool InsertDetail(OrderDetail d)
        {
            string sql = @"INSERT INTO OrderDetails(OrderId, DeviceRequirement, Quantity, UnitPrice)
                           VALUES(@OrderId, @DeviceRequirement, @Quantity, @UnitPrice);";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = d.OrderId;
                cmd.Parameters.Add("@DeviceRequirement", SqlDbType.NVarChar).Value = d.DeviceRequirement;
                cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = d.Quantity;
                cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = d.UnitPrice;

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}