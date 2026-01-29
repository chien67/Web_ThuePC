using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DATN_Web.Models.Entities;

namespace DATN_Web.DataAccesLayer
{
    public class BillDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }

        // Tạo Bill khi kết thúc đơn hàng: Status = 0, PaidDate/PaidUserId = NULL
        public void InsertUnpaid(Bill bill)
        {
            string sql = @"
                INSERT INTO Bills
                (OrderId, RentalAmount, DepositAmount, TotalAmount,
                 PaidDate, PaidUserId, Note, Status, CreatedDate,
                 CustomerType, CustomerName, CompanyName, TaxCode)
                VALUES
                (@OrderId, @RentalAmount, @DepositAmount, @TotalAmount,
                 NULL, NULL, @Note, 0, @CreatedDate,
                 @CustomerType, @CustomerName, @CompanyName, @TaxCode)";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", bill.OrderId);
                cmd.Parameters.AddWithValue("@RentalAmount", bill.RentalAmount);
                cmd.Parameters.AddWithValue("@DepositAmount", bill.DepositAmount);
                cmd.Parameters.AddWithValue("@TotalAmount", bill.TotalAmount);
                cmd.Parameters.AddWithValue("@Note", bill.Note ?? "");

                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value =
                    bill.CreatedDate == DateTime.MinValue ? (object)DateTime.Now : bill.CreatedDate;

                cmd.Parameters.AddWithValue("@CustomerType", bill.CustomerType);
                cmd.Parameters.AddWithValue("@CustomerName", bill.CustomerName ?? "");

                cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar).Value =
                    string.IsNullOrWhiteSpace(bill.CompanyName) ? (object)DBNull.Value : bill.CompanyName;

                cmd.Parameters.Add("@TaxCode", SqlDbType.NVarChar).Value =
                    string.IsNullOrWhiteSpace(bill.TaxCode) ? (object)DBNull.Value : bill.TaxCode;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Thanh toán: Status=1, PaidDate=GETDATE(), PaidUserId=@PaidUserId
        public bool Pay(int billId, int paidUserId)
        {
            string sql = @"
                UPDATE Bills
                SET Status = 1,
                    PaidDate = GETDATE(),
                    PaidUserId = @PaidUserId
                WHERE BillId = @BillId
                  AND Status = 0;";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@BillId", billId);
                cmd.Parameters.AddWithValue("@PaidUserId", paidUserId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Lấy Bill theo OrderId
        public Bill GetByOrderId(int orderId)
        {
            string sql = "SELECT TOP 1 * FROM Bills WHERE OrderId = @OrderId";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return MapBill(reader);
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

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return MapBill(reader);
                }
            }
            return null;
        }

        // Lấy danh sách Bill (đơn kết thúc đã tạo bill thì sẽ thấy, gồm cả Status=0 và 1)
        public List<Bill> GetAll()
        {
            List<Bill> list = new List<Bill>();
            string sql = "SELECT * FROM Bills ORDER BY CreatedDate DESC";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapBill(reader));
                    }
                }
            }
            return list;
        }
        public void InsertPaid(Bill bill)
        {
            string sql = @"
        INSERT INTO Bills
        (OrderId, RentalAmount, DepositAmount, TotalAmount,
         PaidDate, PaidUserId, Note, Status, CreatedDate,
         CustomerType, CustomerName, CompanyName, TaxCode)
        VALUES
        (@OrderId, @RentalAmount, @DepositAmount, @TotalAmount,
         @PaidDate, @PaidUserId, @Note, 1, @CreatedDate,
         @CustomerType, @CustomerName, @CompanyName, @TaxCode)";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", bill.OrderId);
                cmd.Parameters.AddWithValue("@RentalAmount", bill.RentalAmount);
                cmd.Parameters.AddWithValue("@DepositAmount", bill.DepositAmount);
                cmd.Parameters.AddWithValue("@TotalAmount", bill.TotalAmount);

                cmd.Parameters.Add("@PaidDate", SqlDbType.DateTime).Value =
                    (bill.PaidDate == DateTime.MinValue) ? (object)DBNull.Value : bill.PaidDate;

                cmd.Parameters.Add("@PaidUserId", SqlDbType.Int).Value =
                    (bill.PaidUserId <= 0) ? (object)DBNull.Value : bill.PaidUserId;

                cmd.Parameters.AddWithValue("@Note", bill.Note ?? "");

                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value =
                    bill.CreatedDate == DateTime.MinValue ? DateTime.Now : bill.CreatedDate;

                cmd.Parameters.AddWithValue("@CustomerType", bill.CustomerType);
                cmd.Parameters.AddWithValue("@CustomerName", bill.CustomerName ?? "");

                cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar).Value =
                    string.IsNullOrWhiteSpace(bill.CompanyName) ? (object)DBNull.Value : bill.CompanyName;

                cmd.Parameters.Add("@TaxCode", SqlDbType.NVarChar).Value =
                    string.IsNullOrWhiteSpace(bill.TaxCode) ? (object)DBNull.Value : bill.TaxCode;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public bool IsPaidByOrderId(int orderId)
        {
            string sql = "SELECT TOP 1 Status FROM Bills WHERE OrderId=@OrderId";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                conn.Open();
                object v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value) return false;
                return Convert.ToBoolean(v);
            }
        }
        // Map dữ liệu từ DB → Model (FIX NULL PaidDate/PaidUserId)
        private Bill MapBill(SqlDataReader reader)
        {
            return new Bill
            {
                BillId = Convert.ToInt32(reader["BillId"]),
                OrderId = Convert.ToInt32(reader["OrderId"]),
                RentalAmount = Convert.ToDecimal(reader["RentalAmount"]),
                DepositAmount = Convert.ToDecimal(reader["DepositAmount"]),
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),

                //NULL-safe
                PaidDate = reader["PaidDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["PaidDate"]),
                PaidUserId = reader["PaidUserId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PaidUserId"]),

                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),

                Note = reader["Note"] == DBNull.Value ? "" : reader["Note"].ToString(),

                CustomerType = reader["CustomerType"] == DBNull.Value ? (byte)1 : Convert.ToByte(reader["CustomerType"]),
                CustomerName = reader["CustomerName"] == DBNull.Value ? string.Empty : reader["CustomerName"].ToString(),
                CompanyName = reader["CompanyName"] == DBNull.Value ? null : reader["CompanyName"].ToString(),
                TaxCode = reader["TaxCode"] == DBNull.Value ? null : reader["TaxCode"].ToString(),

                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"])
            };
        }
    }
}
