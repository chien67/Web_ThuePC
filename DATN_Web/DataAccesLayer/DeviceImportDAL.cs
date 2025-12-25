using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DATN_Web.Models.Entities;

namespace DATN_Web.DataAccesLayer
{
    public class DeviceImportDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        // Xác nhận nhập hàng
        public int InsertImport(DeviceImport imp)
        {
            string connStr = GetConnectionString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                               INSERT INTO DeviceImport (ModelId, ImportQuantity, Partner, ImportType, Note)
                               VALUES (@ModelId, @ImportQuantity, @Partner, @ImportType, @Note);
                               SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ModelId", imp.ModelId);
                cmd.Parameters.AddWithValue("@ImportQuantity", imp.ImportQuantity);
                cmd.Parameters.AddWithValue("@Partner", (object)imp.Partner ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImportType", imp.ImportType);
                cmd.Parameters.AddWithValue("@Note", (object)imp.Note ?? DBNull.Value);

                conn.Open();
                try
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    // Quan trọng: để bạn nhìn thấy lỗi SQL thực sự
                    throw new Exception("InsertImport failed: " + ex.Message, ex);
                }
            }
        }
    }
}