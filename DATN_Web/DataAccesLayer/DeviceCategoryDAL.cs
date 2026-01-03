using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Design;
using System.Linq;
using System.Web;
using DATN_Web.Models;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.DataAccesLayer
{
    public class DeviceCategoryDAL
    {
        private string GetConnectionString()
        {
            // Lấy chuỗi kết nối từ Web.config/App.config
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        public List<DeviceCategory> GetAllCategory()
        {
            string connStr = GetConnectionString();
            List<DeviceCategory> listCategories = new List<DeviceCategory>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT Id,CategoryName,ModelCount,TotalQuantity,LastUpdated FROM DeviceCategory";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    listCategories.Add(new DeviceCategory
                    {
                        Id = (int)rd["ID"],
                        CategoryName = rd["CategoryName"].ToString(),
                        ModelCount = (int)rd["ModelCount"],
                        TotalQuantity = (int)rd["TotalQuantity"],
                        LastUpdated = (DateTime)rd["LastUpdated"]
                    });
                }
                return listCategories;
            }

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
                                 VALUES (@CategoryName, @ModelCount, @TotalQuanity, @LastUpdated)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@CategoryName", deviceCategory.CategoryName);
                    cmd.Parameters.AddWithValue("@ModelCount", deviceCategory.ModelCount);
                    cmd.Parameters.AddWithValue("@TotalQuanity", deviceCategory.TotalQuantity);
                    cmd.Parameters.AddWithValue("@LastUpdated", deviceCategory.LastUpdated);
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
        public DeviceCategory GetById(int id)
        {
            string connStr = GetConnectionString();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT * FROM DeviceCategory WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    return new DeviceCategory
                    {
                        Id = (int)rd["Id"],
                        CategoryName = rd["CategoryName"].ToString()
                    };
                }
            }
            return null;
        }
        // Tổng hợp số lượng model và số lượng tổng máy
        public List<DeviceCategoryVM> GetCategoryStats()
        {
            var list = new List<DeviceCategoryVM>();
            string connStr = GetConnectionString();
            var sql = @"
            SELECT
                c.Id,
                c.CategoryName,
                COUNT(m.Id)                     AS ModelCount,
                ISNULL(SUM(m.TotalQuantity), 0) AS TotalQuantity,
                c.LastUpdated
            FROM dbo.DeviceCategory c
            LEFT JOIN dbo.DeviceModel m ON m.CategoryId = c.Id
            GROUP BY c.Id, c.CategoryName, c.LastUpdated
            ORDER BY c.Id;";

            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new DeviceCategoryVM
                        {
                            Id = (int)r["Id"],
                            CategoryName = r["CategoryName"].ToString(),
                            ModelCount = (int)r["ModelCount"],
                            TotalQuantity = (int)r["TotalQuantity"],
                            LastUpdated = r["LastUpdated"] == DBNull.Value ? (DateTime?)null : (DateTime)r["LastUpdated"]
                        });
                    }
                }
            }
            return list;
        }
    }
}