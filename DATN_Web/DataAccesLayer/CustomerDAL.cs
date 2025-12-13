using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using DATN_Web.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DATN_Web.DataAccesLayer
{
    public class CustomerDAL
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        public List<Customer> GetAll()
        {
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

    }
}
