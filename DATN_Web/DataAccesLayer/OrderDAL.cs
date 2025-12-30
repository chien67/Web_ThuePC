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
    public class OrderDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        public int InsertOrder(Order o)
        {
            string sql = @"INSERT INTO Orders(CustomerId, CreatedAt, Quantity, DeviceRequirement, DeliveryDate, ReturnDate, RentDays, DeliveryAddress, UnitPrice, Status)
                           VALUES(@CustomerId, GETDATE(), @Quantity, @DeviceRequirement, @DeliveryDate, @ReturnDate, @RentDays, @DeliveryAddress, @UnitPrice, @Status);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = o.CustomerId;
                cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = (object)o.DeliveryDate ?? DBNull.Value;
                cmd.Parameters.Add("@DeviceRequirement", SqlDbType.NVarChar).Value = (object)o.DeviceRequirement ?? DBNull.Value;
                cmd.Parameters.Add("@ReturnDate", SqlDbType.Date).Value = (object)o.ReturnDate ?? DBNull.Value;
                cmd.Parameters.Add("@RentDays", SqlDbType.Int).Value = o.RentDays;
                cmd.Parameters.Add("@DeliveryAddress", SqlDbType.NVarChar).Value = (object)o.DeliveryAddress ?? DBNull.Value;
                cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = o.Quantity;
                // Decimal phải set Precision/Scale để tránh lỗi
                var p = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                p.Precision = 18;
                p.Scale = 2;
                p.Value = o.UnitPrice;

                cmd.Parameters.Add("@Status", SqlDbType.TinyInt).Value = o.Status;

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            var list = new List<Order>();
            string sql = @"SELECT OrderId, CustomerId, CreatedAt, Quantity, DeviceRequirement, DeliveryDate, ReturnDate, RentDays, DeliveryAddress, UnitPrice, Status
                           FROM Orders
                           WHERE CustomerId = @CustomerId
                           ORDER BY CreatedAt DESC;";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Order
                        {
                            OrderId = (int)r["OrderId"],
                            CustomerId = (int)r["CustomerId"],
                            CreatedAt = (DateTime)r["CreatedAt"],
                            DeliveryDate = r["DeliveryDate"] as DateTime?,
                            ReturnDate = r["ReturnDate"] as DateTime?,
                            RentDays = r["RentDays"] == DBNull.Value ? 0 : Convert.ToInt32(r["RentDays"]),
                            DeliveryAddress = r["DeliveryAddress"] as string,
                            DeviceRequirement = r["DeviceRequirement"] as string,
                            UnitPrice = r["UnitPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(r["UnitPrice"]),
                            Quantity = (int)r["Quantity"],
                            Status = Convert.ToByte(r["Status"])
                        });
                    }
                }
            }

            return list;
        }
        public Order GetOrderById(int id)
        {
            string sql = @"SELECT OrderId, CustomerId, CreatedAt, DeviceRequirement, DeliveryDate, ReturnDate,
                          RentDays, Quantity, DeliveryAddress, UnitPrice,DepositAmount, Status
                   FROM Orders
                   WHERE OrderId = @Id";
            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new Order
                    {
                        OrderId = (int)r["OrderId"],
                        CustomerId = (int)r["CustomerId"],
                        CreatedAt = (DateTime)r["CreatedAt"],
                        DeviceRequirement = r["DeviceRequirement"] as string,
                        DeliveryDate = r["DeliveryDate"] as DateTime?,
                        ReturnDate = r["ReturnDate"] as DateTime?,
                        RentDays = r["RentDays"] == DBNull.Value ? 0 : Convert.ToInt32(r["RentDays"]),
                        Quantity = r["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(r["Quantity"]),
                        DeliveryAddress = r["DeliveryAddress"] as string,
                        UnitPrice = r["UnitPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(r["UnitPrice"]),
                        DepositAmount = r["DepositAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(r["DepositAmount"]),
                        Status = Convert.ToByte(r["Status"])
                    };
                }
            }
        }
        public bool UpdateOrder(Order o)
        {
            string sql = @"UPDATE Orders
                   SET CustomerId = @CustomerId,
                       DeviceRequirement = @DeviceRequirement,
                       DeliveryDate = @DeliveryDate,
                       ReturnDate = @ReturnDate,
                       RentDays = @RentDays,
                       Quantity = @Quantity,
                       DeliveryAddress = @DeliveryAddress,
                       UnitPrice = @UnitPrice,
                       DepositAmount = @DepositAmount,
                       Status = @Status
                   WHERE OrderId = @OrderId";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = o.OrderId;
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = o.CustomerId;

                var req = cmd.Parameters.Add("@DeviceRequirement", SqlDbType.NVarChar, 1000);
                req.Value = string.IsNullOrWhiteSpace(o.DeviceRequirement) ? (object)DBNull.Value : o.DeviceRequirement.Trim();

                cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = (object)o.DeliveryDate ?? DBNull.Value;
                cmd.Parameters.Add("@ReturnDate", SqlDbType.Date).Value = (object)o.ReturnDate ?? DBNull.Value;
                cmd.Parameters.Add("@RentDays", SqlDbType.Int).Value = o.RentDays;

                cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = o.Quantity;

                var addr = cmd.Parameters.Add("@DeliveryAddress", SqlDbType.NVarChar, 500);
                addr.Value = string.IsNullOrWhiteSpace(o.DeliveryAddress) ? (object)DBNull.Value : o.DeliveryAddress.Trim();

                var p = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                p.Precision = 18; p.Scale = 2;
                p.Value = o.UnitPrice;

                var dep = cmd.Parameters.Add("@DepositAmount", SqlDbType.Decimal);
                dep.Precision = 18; p.Scale = 2;
                dep.Value = o.DepositAmount;

                var note = cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 2000);
                note.Value = string.IsNullOrWhiteSpace(o.Note) ? (object)DBNull.Value : o.Note.Trim();

                cmd.Parameters.Add("@Status", SqlDbType.TinyInt).Value = o.Status;

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}