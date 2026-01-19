using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using DATN_Web.Models.DTO;
using DATN_Web.Models.ViewModels;
using DATN_Web.Models.Entities;
using System.Web.Security;
using DATN_Web.Models.Enum;

namespace DATN_Web.DataAccesLayer
{
    public class CustomerDeviceDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        // Lấy danh mục để đổ dropdown
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
        public List<User> GetDeliveryUsers()
        {
            var list = new List<User>();

            const string sql = @"SELECT UserId, FullName FROM [Users] WHERE IsActive = 1 AND Role = 1";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            FullName = reader.GetString(1)
                        });
                    }
                }
            }

            return list;
        }

        // Lấy model theo danh mục (để Ajax đổ dropdown Model/Cấu hình + tồn kho)
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

        // Chức năng XUẤT thiết bị cho khách
        public void AssignDeviceToCustomer(int customerId, int modelId, int quantity, int DeliveryUserId)
        {
            if (customerId <= 0) throw new Exception("CustomerId không hợp lệ.");
            if (modelId <= 0) throw new Exception("ModelId không hợp lệ.");
            if (quantity <= 0) throw new Exception("Số lượng phải > 0.");
            if (DeliveryUserId <= 0) throw new Exception("Người nhận không hợp lệ");

            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // Check tồn kho (khóa dòng để tránh 2 người xuất cùng lúc)
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
                            INSERT INTO CustomerDevices(CustomerId, ModelId, DeliveryUserId, DeliveryDate, Quantity, Status)
                            VALUES (@CustomerId, @ModelId, @DeliveryUserId, @DeliveryDate, @Quantity, 1)";

                        using (var cmd = new SqlCommand(sqlInsert, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            cmd.Parameters.AddWithValue("@ModelId", modelId);
                            cmd.Parameters.AddWithValue("@DeliveryUserId", DeliveryUserId);
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
        // 1. Danh sách thiết bị chờ nhận của 1 nhân viên
        public List<CustomerDevice> GetWaitingReceiveByDeliveryUser(int deliveryUserId)
        {
            var list = new List<CustomerDevice>();

            const string sql = @"
                    SELECT Id, CustomerId, ModelId, Quantity, DeliveryDate, Status
                    FROM CustomerDevices
                    WHERE DeliveryUserId = @DeliveryUserId
                      AND Status = @Status";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DeliveryUserId", deliveryUserId);
                cmd.Parameters.AddWithValue("@Status",
                    (int)CustomerDeviceStatus.WaitingReceive);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CustomerDevice
                        {
                            Id = (int)rd["Id"],
                            CustomerId = (int)rd["CustomerId"],
                            ModelId = (int)rd["ModelId"],
                            Quantity = (int)rd["Quantity"],
                            DeliveryDate = rd["DeliveryDate"] == DBNull.Value
                               ? (DateTime?)null
                               : Convert.ToDateTime(rd["DeliveryDate"]),
                            Status = (CustomerDeviceStatus)Convert.ToInt32(rd["Status"])
                        });
                    }
                }
            }
            return list;
        }

        // 2. Nhân viên xác nhận nhận & giao
        public void ConfirmByDeliveryUser(int customerDeviceId, int deliveryUserId)
        {
            const string sql = @"UPDATE CustomerDevices
            SET Status = 2 WHERE Id = @Id
              AND DeliveryUserId = @DeliveryUserId
              AND Status = 1";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", customerDeviceId);
                cmd.Parameters.AddWithValue("@DeliveryUserId", deliveryUserId);

                conn.Open();
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                    throw new Exception(
                        "Không thể xác nhận (sai người nhận hoặc sai trạng thái).");
            }
        }
        // Trả về Ds ra ngoài bảng detailcustomer
        public List<CustomerDeviceRowDto> GetDevicesByCustomerId(int customerId, bool onlyInUse = true)
        {
            // onlyInUse=true => chỉ lấy Status=1 (đang dùng)
            const string sql = @"SELECT 
                                     cd.Id,
                                     cd.DeliveryDate,
                                     cd.Quantity,
                                     cd.Status,
                                     cd.DeliveryUserId,
                                     dc.CategoryName,
                                     dm.ModelName,
                                     dm.Configuration
                                 FROM CustomerDevices cd
                                 INNER JOIN DeviceModel dm ON cd.ModelId = dm.Id
                                 INNER JOIN DeviceCategory dc ON dm.CategoryId = dc.Id
                                 WHERE cd.CustomerId = @CustomerId
                                 AND (@OnlyInUse = 0 OR cd.Status <> 3)
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
                            Status = (CustomerDeviceStatus)Convert.ToInt32(rd["Status"]),
                            DeliveryUserId = rd["DeliveryUserId"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["DeliveryUserId"]),
                            CategoryName = rd["CategoryName"].ToString(),
                            ModelName = rd["ModelName"].ToString(),
                            Configuration = rd["Configuration"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        // thu hồi
        public ReturnDeviceVM GetReturnFormData(int customerDeviceId)
        {
            const string sql = @"SELECT 
                                    cd.Id AS CustomerDeviceId,
                                    cd.CustomerId,
                                    cd.Quantity AS InUseQuantity,
                                    dc.CategoryName,
                                    dm.ModelName,
                                    dm.Configuration
                                FROM CustomerDevices cd
                                INNER JOIN DeviceModel dm ON cd.ModelId = dm.Id
                                INNER JOIN DeviceCategory dc ON dm.CategoryId = dc.Id
                                WHERE cd.Id = @Id AND cd.Status = 2";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", customerDeviceId);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;

                    return new ReturnDeviceVM
                    {
                        CustomerDeviceId = Convert.ToInt32(rd["CustomerDeviceId"]),
                        CustomerId = Convert.ToInt32(rd["CustomerId"]),
                        InUseQuantity = Convert.ToInt32(rd["InUseQuantity"]),
                        CategoryName = rd["CategoryName"].ToString(),
                        ModelText = rd["ModelName"].ToString() + " - Cấu hình: " + rd["Configuration"].ToString()
                    };
                }
            }
        }
        // Thu hồi về 1 phần
        public void ReturnDevicePartial(int customerDeviceId, int returnQty)
        {
            if (customerDeviceId <= 0) throw new Exception("Id thu hồi không hợp lệ.");
            if (returnQty <= 0) throw new Exception("Số lượng thu hồi phải > 0.");

            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // Khóa dòng đang dùng để lấy ModelId + Quantity + CustomerId
                        const string sqlGet = @"SELECT CustomerId, ModelId, Quantity, Status
                                                FROM CustomerDevices WITH (UPDLOCK, ROWLOCK)
                                                WHERE Id = @Id";

                        int customerId, modelId, inUseQty;
                        byte status;

                        using (var cmd = new SqlCommand(sqlGet, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", customerDeviceId);
                            using (var rd = cmd.ExecuteReader())
                            {
                                if (!rd.Read())
                                    throw new Exception("Không tìm thấy dòng thiết bị.");

                                customerId = Convert.ToInt32(rd["CustomerId"]);
                                modelId = Convert.ToInt32(rd["ModelId"]);
                                inUseQty = Convert.ToInt32(rd["Quantity"]);
                                status = Convert.ToByte(rd["Status"]);
                            }
                        }

                        var deviceStatus = (CustomerDeviceStatus)status;

                        if (deviceStatus == CustomerDeviceStatus.WaitingReceive)
                            throw new Exception("Nhân viên chưa giao máy cho khách.");

                        if (deviceStatus != CustomerDeviceStatus.ReceivedAndDelivered)
                            throw new Exception("Thiết bị không ở trạng thái cho phép thu hồi.");

                        if (returnQty > inUseQty)
                            throw new Exception("Số lượng thu hồi vượt số đang sử dụng.");

                        // Update dòng đang dùng: giảm Quantity hoặc set Status=2 nếu về 0
                        int remain = inUseQty - returnQty;

                        if (remain > 0)
                        {
                            const string sqlUpdateInUse = @"UPDATE CustomerDevices
                                                            SET Quantity = @Remain
                                                            WHERE Id = @Id AND Status = 2";

                            using (var cmd = new SqlCommand(sqlUpdateInUse, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@Remain", remain);
                                cmd.Parameters.AddWithValue("@Id", customerDeviceId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // thu hồi hết: set Status=3 + ReturnDate
                            const string sqlClose = @"UPDATE CustomerDevices
                                                      SET Status = 3, ReturnDate = @ReturnDate
                                                      WHERE Id = @Id AND Status = 2";

                            using (var cmd = new SqlCommand(sqlClose, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@ReturnDate", DateTime.Today);
                                cmd.Parameters.AddWithValue("@Id", customerDeviceId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        //Cập nhật kho: cộng tồn, trừ đang dùng
                        const string sqlStock = @"UPDATE DeviceModel
                                                  SET InStockQuantity = InStockQuantity + @Qty,
                                                      InUseQuantity = CASE WHEN InUseQuantity >= @Qty THEN InUseQuantity - @Qty ELSE 0 END
                                                  WHERE Id = @ModelId";

                        using (var cmd = new SqlCommand(sqlStock, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Qty", returnQty);
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
        // Lịch sử thu hồi
        public List<CustomerDeviceRowDto> GetReturnHistoryByCustomerId(int customerId)
        {
            const string sql = @"SELECT 
                                     cd.Id,
                                     cd.ReturnDate AS DeliveryDate,
                                     cd.Quantity,
                                     cd.Status,
                                     dc.CategoryName,
                                     dm.ModelName,
                                     dm.Configuration
                                 FROM CustomerDevices cd
                                 INNER JOIN DeviceModel dm ON cd.ModelId = dm.Id
                                 INNER JOIN DeviceCategory dc ON dm.CategoryId = dc.Id
                                 WHERE cd.CustomerId = @CustomerId
                                   AND cd.Status = 3
                                 ORDER BY cd.ReturnDate DESC, cd.Id DESC;";

            var list = new List<CustomerDeviceRowDto>();

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

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
                            Status = (CustomerDeviceStatus)Convert.ToInt32(rd["Status"]),
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
