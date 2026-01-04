using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using DATN_Web.Models.DTO;

namespace DATN_Web.DataAccesLayer
{
    public class CustomerDeviceDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        // 1) Lấy danh mục để đổ dropdown
        public List<(int Id, string Name)> GetCategories()
        {
            const string sql = @"SELECT Id, CategoryName FROM DeviceCategory ORDER BY CategoryName";

            var list = new List<(int, string)>();

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add((Convert.ToInt32(rd["Id"]), rd["CategoryName"].ToString()));
                    }
                }
            }

            return list;
        }

        // 2) Lấy model theo danh mục (để Ajax đổ dropdown Model/Cấu hình + tồn kho)
        public List<DeviceModelOptionDto> GetModelsByCategory(int categoryId)
        {
            const string sql = @"
                SELECT Id, ModelName, Configuration, InStockQuantity
                FROM DeviceModel
                WHERE CategoryId = @CategoryId
                ORDER BY ModelName";

            var list = new List<DeviceModelOptionDto>();

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var modelName = rd["ModelName"]?.ToString() ?? "";
                        var config = rd["Configuration"]?.ToString() ?? "";

                        list.Add(new DeviceModelOptionDto
                        {
                            Id = Convert.ToInt32(rd["Id"]),
                            Text = $"{modelName} (Cấu hình: {config})",
                            Stock = Convert.ToInt32(rd["InStockQuantity"])
                        });
                    }
                }
            }

            return list;
        }

        // 3) Chức năng XUẤT thiết bị cho khách (transaction)
        //    - insert vào CustomerDevices
        //    - update tồn kho trong DeviceModel
        public void AssignDeviceToCustomer(int customerId, int modelId, int quantity)
        {
            if (customerId <= 0) throw new Exception("CustomerId không hợp lệ.");
            if (modelId <= 0) throw new Exception("ModelId không hợp lệ.");
            if (quantity <= 0) throw new Exception("Số lượng phải > 0.");

            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // 3.1) Check tồn kho (khóa dòng để tránh 2 người xuất cùng lúc)
                        const string sqlCheck = @"
                            SELECT InStockQuantity
                            FROM DeviceModel WITH (UPDLOCK, ROWLOCK)
                            WHERE Id = @ModelId";

                        int stock;
                        using (var cmd = new SqlCommand(sqlCheck, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@ModelId", modelId);

                            var obj = cmd.ExecuteScalar();
                            if (obj == null) throw new Exception("Model không tồn tại.");

                            stock = Convert.ToInt32(obj);
                        }

                        if (stock < quantity)
                            throw new Exception("Không đủ tồn kho.");

                        // 3.2) Insert vào CustomerDevices
                        const string sqlInsert = @"
                            INSERT INTO CustomerDevices(CustomerId, ModelId, DeliveryDate, Quantity, Status)
                            VALUES (@CustomerId, @ModelId, @DeliveryDate, @Quantity, 1)";

                        using (var cmd = new SqlCommand(sqlInsert, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            cmd.Parameters.AddWithValue("@ModelId", modelId);
                            cmd.Parameters.AddWithValue("@DeliveryDate", DateTime.Today);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);

                            cmd.ExecuteNonQuery();
                        }

                        // 3.3) Update kho: trừ tồn, cộng đang dùng
                        const string sqlUpdate = @"
                            UPDATE DeviceModel
                            SET InStockQuantity = InStockQuantity - @Quantity,
                                InUseQuantity = InUseQuantity + @Quantity
                            WHERE Id = @ModelId";

                        using (var cmd = new SqlCommand(sqlUpdate, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@ModelId", modelId);

                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<CustomerDeviceRowDto> GetDevicesByCustomerId(int customerId, bool onlyInUse = true)
        {
            // onlyInUse=true => chỉ lấy Status=1 (đang dùng)
            const string sql = @"SELECT 
                                     cd.Id,
                                     cd.DeliveryDate,
                                     cd.Quantity,
                                     cd.Status,
                                     dc.CategoryName,
                                     dm.ModelName,
                                     dm.Configuration
                                 FROM CustomerDevices cd
                                 INNER JOIN DeviceModel dm ON cd.ModelId = dm.Id
                                 INNER JOIN DeviceCategory dc ON dm.CategoryId = dc.Id
                                 WHERE cd.CustomerId = @CustomerId
                                   AND (@OnlyInUse = 0 OR cd.Status = 1)
                                 ORDER BY cd.DeliveryDate DESC, cd.Id DESC;";

            var list = new List<CustomerDeviceRowDto>();

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                cmd.Parameters.AddWithValue("@OnlyInUse", onlyInUse ? 1 : 0);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CustomerDeviceRowDto
                        {
                            Id = Convert.ToInt32(rd["Id"]),
                            DeliveryDate = rd["DeliveryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rd["DeliveryDate"]),
                            Quantity = Convert.ToInt32(rd["Quantity"]),
                            Status = Convert.ToByte(rd["Status"]),
                            CategoryName = rd["CategoryName"].ToString(),
                            ModelName = rd["ModelName"].ToString(),
                            Configuration = rd["Configuration"].ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}
