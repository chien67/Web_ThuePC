using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using DATN_Web.Models;
using DATN_Web.Models.Entities;

namespace DATN_Web.DataAccesLayer
{
    public class DeviceModelDAL
    {
        private string GetConnectionString()
        {
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
                var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    list.Add(new DeviceModel
                    {
                        Id = (int)rd["Id"],
                        CategoryId = (int)rd["CategoryId"],
                        ModelName = rd["ModelName"].ToString(),
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
        public int CreateDeviceModel(DeviceModel model)
        {
            string connStr = GetConnectionString();
            int newId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    // INSERT và lấy lại ModelId mới bằng SCOPE_IDENTITY()
                    string sql = @"
                    INSERT INTO DeviceModel (CategoryId,
                    ModelName,
                    Configuration,
                    TotalQuantity,
                    InStockQuantity,
                    InUseQuantity,
                    BrokenQuantity,
                    LastUpdatedAt)
                    VALUES (@CategoryId,
                    @ModelName,
                    @Configuration,
                    0, 0, 0, 0,
                    GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                    cmd.Parameters.AddWithValue("@ModelName", model.ModelName);
                    cmd.Parameters.AddWithValue("@Configuration", model.Configuration);

                    conn.Open();

                    // Dùng ExecuteScalar() để lấy ID mới
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        newId = Convert.ToInt32(result);
                    }
                }
                return newId; // Trả về ID mới
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                throw new Exception("Lỗi DAL khi tạo Model: " + ex.Message);
            }
        }
        public int DeleteDeviceModel(int modelId)
        {
            string connStr = GetConnectionString();
            int rowsAffected = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    // Lệnh SQL DELETE
                    string sql = "DELETE FROM DeviceModel WHERE Id = @Id";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // Thêm tham số ModelId
                    cmd.Parameters.AddWithValue("@Id", modelId);

                    conn.Open();

                    // ExecuteNonQuery trả về số dòng bị ảnh hưởng
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                return rowsAffected; // Sẽ là 1 nếu xóa thành công, 0 nếu không tìm thấy ID
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và ném ra Exception (Hoặc xử lý lỗi ngoại lệ DB)
                throw new Exception("Lỗi DAL khi xóa Model: " + ex.Message);
            }
        }
        public bool UpdateQuantities(int modelId, int quantity)
        {
            string connStr = GetConnectionString();

            string sql = @"
            UPDATE DeviceModel 
            SET TotalQuantity = TotalQuantity + @Quantity, 
            InStockQuantity = InStockQuantity + @Quantity, 
            LastUpdatedAt = GETDATE() 
            WHERE Id = @ModelId";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@ModelId", modelId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        public DeviceModel GetModelById(int modelId)
        {
            string connStr = GetConnectionString();
            DeviceModel model = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT * FROM DeviceModel WHERE Id = @ModelId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ModelId", modelId);

                conn.Open();
                SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    // Nếu tìm thấy bản ghi, khởi tạo đối tượng DeviceModel
                    model = new DeviceModel
                    {
                        Id = (int)rd["Id"],
                        CategoryId = (int)rd["CategoryId"],
                        ModelName = rd["ModelName"].ToString(),
                        Configuration = rd["Configuration"].ToString(),
                        TotalQuantity = (int)rd["TotalQuantity"],
                        InStockQuantity = (int)rd["InStockQuantity"],
                        InUseQuantity = (int)rd["InUseQuantity"],
                        BrokenQuantity = (int)rd["BrokenQuantity"],
                        LastUpdatedAt = (DateTime)rd["LastUpdatedAt"]
                    };
                }
            }
            return model;
        }
        
    }
}