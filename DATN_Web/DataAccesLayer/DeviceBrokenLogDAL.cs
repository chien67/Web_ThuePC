using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DATN_Web.Models.DTO;
using DATN_Web.Models.Entities;
using DATN_Web.Models.Enum;

namespace DATN_Web.DataAccesLayer
{
    public class DeviceBrokenLogDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        // Lấy danh sách thiết bị hỏng
        public List<DeviceBrokenLogRowDto> GetAll()
        {
            var list = new List<DeviceBrokenLogRowDto>();

            const string sql = @"SELECT bl.BrokenLogId,
                                        bl.Quantity,
                                        bl.BrokenReason,
                                        bl.EstimatedCost,
                                        bl.CreatedAt,
                                        dm.ModelName,
                                        dm.Configuration,
                                        c.CustomerName,
                                        c.RepresentativeName
                                    FROM DeviceBrokenLogs bl
                                    INNER JOIN DeviceModel dm ON bl.ModelId = dm.Id
                                    LEFT JOIN Customers c ON bl.CustomerId = c.CustomerId
                                    ORDER BY bl.CreatedAt DESC";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DeviceBrokenLogRowDto
                        {
                            BrokenLogId = (int)rd["BrokenLogId"],
                            Quantity = (int)rd["Quantity"],
                            BrokenReason = rd["BrokenReason"].ToString(),
                            EstimatedCost = rd["EstimatedCost"] == DBNull.Value
                                ? (decimal?)null
                                : (decimal)rd["EstimatedCost"],

                            ModelName = rd["ModelName"].ToString(),
                            Configuration = rd["Configuration"].ToString(),

                            CustomerName = rd["CustomerName"]?.ToString(),
                            RepresentativeName = rd["RepresentativeName"]?.ToString(),

                            CreatedAt = (DateTime)rd["CreatedAt"]
                        });
                    }
                }
            }
            return list;
        }
        // Lấy ra 1 thiết bị hỏng
        public DeviceBrokenLogRowDto GetById(int brokenLogId)
        {
            const string sql = @"
        SELECT 
            bl.BrokenLogId,
            bl.Quantity,
            bl.BrokenReason,
            bl.EstimatedCost,
            bl.CostNote,
            bl.CreatedAt,

            dm.ModelName,
            dm.Configuration,

            c.CustomerName,
            c.RepresentativeName
        FROM DeviceBrokenLogs bl
        INNER JOIN DeviceModel dm ON bl.ModelId = dm.Id
        LEFT JOIN Customers c ON bl.CustomerId = c.CustomerId
        WHERE bl.BrokenLogId = @Id";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", brokenLogId);
                conn.Open();

                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;

                    return new DeviceBrokenLogRowDto
                    {
                        BrokenLogId = (int)rd["BrokenLogId"],
                        Quantity = (int)rd["Quantity"],
                        BrokenReason = rd["BrokenReason"].ToString(),
                        EstimatedCost = rd["EstimatedCost"] == DBNull.Value
                                            ? (decimal?)null
                                            : (decimal)rd["EstimatedCost"],
                        CostNote = rd["CostNote"]?.ToString(),
                        CreatedAt = (DateTime)rd["CreatedAt"],

                        ModelName = rd["ModelName"].ToString(),
                        Configuration = rd["Configuration"].ToString(),

                        CustomerName = rd["CustomerName"]?.ToString(),
                        RepresentativeName = rd["RepresentativeName"]?.ToString()
                    };
                }
            }
        }
        public void Create(DeviceBrokenLog log)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert log
                        string sqlInsert = @"
                        INSERT INTO DeviceBrokenLogs
                        (ModelId, CustomerId, Quantity, BrokenReason, Status, CreatedBy, CreatedAt)
                        VALUES
                        (@ModelId, @CustomerId, @Quantity, @BrokenReason, 0, @CreatedBy, GETDATE())";

                        using (var cmd = new SqlCommand(sqlInsert, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@ModelId", log.ModelId);
                            cmd.Parameters.AddWithValue("@CustomerId", (object)log.CustomerId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Quantity", log.Quantity);
                            cmd.Parameters.AddWithValue("@BrokenReason", log.BrokenReason);
                            cmd.Parameters.AddWithValue("@CreatedBy", log.CreatedBy);
                            cmd.ExecuteNonQuery();
                        }

                        // Update BrokenQuantity
                        string sqlUpdate = @"
                        UPDATE DeviceModel
                        SET BrokenQuantity = BrokenQuantity + @Qty,
                            LastUpdatedAt = GETDATE()
                        WHERE Id = @ModelId";

                        using (var cmd = new SqlCommand(sqlUpdate, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Qty", log.Quantity);
                            cmd.Parameters.AddWithValue("@ModelId", log.ModelId);
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
        public void UpdateRepairCost(int brokenLogId, decimal cost, string note)
        {
            string sql = @"
        UPDATE DeviceBrokenLogs
        SET EstimatedCost = @Cost,
            CostNote = @Note,
            Status = 1
        WHERE BrokenLogId = @Id";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Cost", cost);
                cmd.Parameters.AddWithValue("@Note", note ?? "");
                cmd.Parameters.AddWithValue("@Id", brokenLogId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}