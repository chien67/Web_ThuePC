using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using DATN_Web.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DATN_Web.DataAccesLayer
{
    public class CustomerDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        public List<Customer> GetAll()
        {
            string connStr = GetConnectionString();
            List<Customer> list = new List<Customer>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT CustomerId,CustomerType,CustomerName,RepresentativeName,TaxCode,Address,Phone,Email,CustomerNote FROM Customers";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    list.Add(new Customer
                    {
                        CustomerId = (int)rd["CustomerId"],
                        CustomerType = (byte)rd["CustomerType"],
                        CustomerName = rd["CustomerName"].ToString(),
                        RepresentativeName = rd["RepresentativeName"].ToString(),
                        TaxCode = rd["TaxCode"].ToString(),
                        Address = rd["Address"].ToString(),
                        Phone = rd["Phone"].ToString(),
                        Email = rd["Email"].ToString(),
                        CustomerNote = rd["CustomerNote"].ToString(),
                    });
                }
                return list;
            }
        }
        public bool CreateCustomers(Customer c)
        {
            string connStr = GetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {

                    string sql = @"INSERT INTO Customers
                                   (CustomerType,CustomerName,RepresentativeName,TaxCode,Address,Phone,Email,CustomerNote)
                             VALUES
                             (
                                   @CustomerType,@CustomerName,@RepresentativeName,@TaxCode,@Address,@Phone,@Email,@CustomerNote
                             )";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@CustomerType", c.CustomerType);
                    cmd.Parameters.AddWithValue("@CustomerName", c.CustomerName ?? "");
                    cmd.Parameters.AddWithValue("@RepresentativeName", c.RepresentativeName ?? "");
                    cmd.Parameters.AddWithValue("@TaxCode", c.TaxCode ?? "");
                    cmd.Parameters.AddWithValue("@Address", c.Address ?? "");
                    cmd.Parameters.AddWithValue("@Phone", c.Phone ?? "");
                    cmd.Parameters.AddWithValue("@Email", c.Email ?? "");
                    cmd.Parameters.AddWithValue("@CustomerNote", c.CustomerNote ?? "");
                    //Console.WriteLine(sql);
                    conn.Open();
                    int row = cmd.ExecuteNonQuery();
                    return row > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi Insert " + ex.Message);
            }
        }
        public Customer GetById(int customerId)
        {
            string sql = @"SELECT * FROM Customers WHERE CustomerId = @CustomerId";
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                conn.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;

                    return new Customer
                    {
                        CustomerId = (int)rd["CustomerId"],
                        CustomerType = (byte)rd["CustomerType"],
                        CustomerName = rd["CustomerName"].ToString(),
                        RepresentativeName = rd["RepresentativeName"].ToString(),
                        TaxCode = rd["TaxCode"].ToString(),
                        Address = rd["Address"].ToString(),
                        Phone = rd["Phone"].ToString(),
                        Email = rd["Email"].ToString(),
                        CustomerNote = rd["CustomerNote"].ToString()
                    };
                }
            }
        }
        public int UpdateCustomer(Customer c)
        {
            const string sql = @"UPDATE dbo.Customers
                                 SET CustomerType = @CustomerType,
                                     CustomerName = @CustomerName,
                                     RepresentativeName = @RepresentativeName,
                                     TaxCode = @TaxCode,
                                     Address = @Address,
                                     Phone = @Phone,
                                     Email = @Email,
                                     CustomerNote = @CustomerNote
                                 WHERE CustomerId = @CustomerId;";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = c.CustomerId;
                cmd.Parameters.Add("@CustomerType", SqlDbType.TinyInt).Value = c.CustomerType;

                cmd.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 500)
                   .Value = (object)c.CustomerName ?? DBNull.Value;

                cmd.Parameters.Add("@RepresentativeName", SqlDbType.NVarChar, 150)
                   .Value = (object)c.RepresentativeName ?? DBNull.Value;

                cmd.Parameters.Add("@TaxCode", SqlDbType.VarChar, 20)
                   .Value = (object)c.TaxCode ?? DBNull.Value;

                cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 500)
                   .Value = (object)c.Address ?? DBNull.Value;

                cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20)
                   .Value = (object)c.Phone ?? DBNull.Value;

                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100)
                   .Value = (object)c.Email ?? DBNull.Value;

                cmd.Parameters.Add("@CustomerNote", SqlDbType.NVarChar, 1000)
                   .Value = (object)c.CustomerNote ?? DBNull.Value;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public bool DeleteCustomer(int customerId)
        {
            const string sql = @"DELETE FROM dbo.Customers WHERE CustomerId = @CustomerId;";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public bool HasOrdersByCustomerId(int customerId)
        {
            const string sql = "SELECT COUNT(1) FROM Orders WHERE CustomerId = @CustomerId";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
