using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Design;
using System.Linq;
using System.Web;
using DATN_Web.Models;

namespace DATN_Web.DataAccesLayer
{
    public class DeviceCategoryDAL
    {
        private string GetConnectionString()
        {
            // Lấy chuỗi kết nối từ Web.config/App.config
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        public bool IsCategoryNameExist(string categoryName)
        {
            string connStr = GetConnectionString();
            string sql = "SELECT COUNT(Id) FROM dbo.DeviceCategory WHERE CategoryName = @CategoryName";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

        }
    
        public bool CreateDeviceCategory(DeviceCategory deviceCategory)
        {
            string connStr = GetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = @"INSERT INTO DeviceCategory (CategoryName, ModelCount, TotalQuantity, LastUpdated)
                                 VALUES (CategoryName, ModelCount, TotalQuantity, LastUpdated)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue(@"CategoryName", deviceCategory.CategoryName);
                    cmd.Parameters.AddWithValue(@"ModelCount", deviceCategory.ModelCount);
                    cmd.Parameters.AddWithValue(@"TotalQuanity", deviceCategory.TotalQuanity);
                    cmd.Parameters.AddWithValue(@"LastUpdated", deviceCategory.LastUpdated);
                    conn.Open();
                    int row = cmd.ExecuteNonQuery();
                    return row > 0;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi Insert" + ex.Message);
            }
        }
        
    }
}
