using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DATN_Web.Models;

namespace DATN_Web.DataAccesLayer
{
    public class DeviceModelDAL
    {
        private string GetConnectionString()
        {
            // Lấy chuỗi kết nối từ Web.config/App.config
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        public List<DeviceModel> GetByCategoryId(int categoryId)
        {
            string connStr = GetConnectionString();
            var list = new List<DeviceModel>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"Select * from DeviceModel where CategoryId = @CategoryId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                conn.Open();
                var rd =cmd.ExecuteReader();
                while (rd.Read()) {
                    list.Add(new DeviceModel
                    {
                        Id = (int)rd["Id"],
                        ModelName= rd["ModelName"].ToString(),
                        Configuration = rd["Configuration"].ToString(),
                        TotalQuantity = (int)rd["TotalQuantity"],
                        InStockQuantity = (int)rd["InStockQuantity"],
                        InUseQuantity = (int)rd["InUseQuantity"],
                        BrokenQuantity = (int)rd["BrokenQuantity"],
                        LastUpdatedAt = (DateTime)rd["LastUpdatedAt"]
                    });
                }
            }
            return list;
        }
        public bool CreateDeviceModel(DeviceModel deviceModel)
        {
            string connStr = GetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = @"
                INSERT INTO DeviceModel
                (
                    CategoryId,
                    ModelName,
                    Configuration,
                    TotalQuantity,
                    InStockQuantity,
                    InUseQuantity,
                    BrokenQuantity,
                    LastUpdatedAt
                )
                VALUES
                (
                    @CategoryId,
                    @ModelName,
                    @Configuration,
                    0, 0, 0, 0,
                    GETDATE()
                )";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@CategoryId", deviceModel.CategoryId);
                    cmd.Parameters.AddWithValue("@ModelName", deviceModel.ModelName);
                    cmd.Parameters.AddWithValue("@Configuration", deviceModel.Configuration);
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