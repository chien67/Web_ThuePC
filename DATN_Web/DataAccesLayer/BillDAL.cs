using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DATN_Web.Models.Entities;

namespace DATN_Web.DataAccesLayer
{
    public class BillDAL
    {
        private string GetConnectionString() { return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString; }
        // Thêm Bill
        public void Insert(Bill bill)
        {
            string sql = @"
                INSERT INTO Bills
                (OrderId, RentalAmount, DepositAmount, TotalAmount,
                 PaidDate, PaidUserId, Note)
                VALUES
                (@OrderId, @RentalAmount, @DepositAmount, @TotalAmount,
                 @PaidDate, @PaidUserId, @Note)";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", bill.OrderId);
                cmd.Parameters.AddWithValue("@RentalAmount", bill.RentalAmount);
                cmd.Parameters.AddWithValue("@DepositAmount", bill.DepositAmount);
                cmd.Parameters.AddWithValue("@TotalAmount", bill.TotalAmount);
                cmd.Parameters.AddWithValue("@PaidDate", bill.PaidDate);
                cmd.Parameters.AddWithValue("@PaidUserId", bill.PaidUserId);
                cmd.Parameters.AddWithValue("@Note", bill.Note ?? "");

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Lấy Bill theo OrderId
        public Bill GetByOrderId(int orderId)
        {
            string sql = "SELECT * FROM Bills WHERE OrderId = @OrderId";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapBill(reader);
                }
            }
            return null;
        }

        // Lấy Bill theo BillId
        public Bill GetById(int billId)
        {
            string sql = "SELECT * FROM Bills WHERE BillId = @BillId";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@BillId", billId);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapBill(reader);
                }
            }
            return null;
        }

        // Lấy danh sách Bill
        public List<Bill> GetAll()
        {
            List<Bill> list = new List<Bill>();
            string sql = "SELECT * FROM Bills ORDER BY PaidDate DESC";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(MapBill(reader));
                }
            }
            return list;
        }

        // Map dữ liệu từ DB → Model
        private Bill MapBill(SqlDataReader reader)
        {
            return new Bill
            {
                BillId = Convert.ToInt32(reader["BillId"]),
                OrderId = Convert.ToInt32(reader["OrderId"]),
                RentalAmount = Convert.ToDecimal(reader["RentalAmount"]),
                DepositAmount = Convert.ToDecimal(reader["DepositAmount"]),
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                PaidDate = Convert.ToDateTime(reader["PaidDate"]),
                PaidUserId = Convert.ToInt32(reader["PaidUserId"]),
                Note = reader["Note"]?.ToString(),
                CustomerType = reader["CustomerType"] == DBNull.Value? (byte)1: Convert.ToByte(reader["CustomerType"]),

                CustomerName = reader["CustomerName"] == DBNull.Value? string.Empty : reader["CustomerName"].ToString(),

                CompanyName = reader["CompanyName"] == DBNull.Value? null : reader["CompanyName"].ToString(),

                TaxCode = reader["TaxCode"] == DBNull.Value? null : reader["TaxCode"].ToString(),
                CreatedDate = reader["CreatedDate"] == DBNull.Value
                                ? DateTime.Now
                                : Convert.ToDateTime(reader["CreatedDate"])

            };
        }
    }
}