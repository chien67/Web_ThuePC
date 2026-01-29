using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.DataAccesLayer
{
    public class WorkTaskDAL
    {
        private string GetConnectionString()
            => ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        public List<DailyTaskRow> GetManualTasksByDate(DateTime date)
        {
            var list = new List<DailyTaskRow>();
            string sql = @"
            SELECT TaskId, Title, Description, DueDate, Priority, IsDone
            FROM WorkTasks
            WHERE DueDate = @DueDate
            ORDER BY IsDone, Priority ASC, TaskId DESC";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DueDate", date.Date);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DailyTaskRow
                        {
                            Source = "MANUAL",
                            TaskId = Convert.ToInt32(rd["TaskId"]),
                            Title = rd["Title"].ToString(),
                            Description = rd["Description"] == DBNull.Value ? "" : rd["Description"].ToString(),
                            DueDate = Convert.ToDateTime(rd["DueDate"]),
                            Priority = Convert.ToByte(rd["Priority"]),
                            IsDone = Convert.ToBoolean(rd["IsDone"]),
                            Link = null
                        });
                    }
                }
            }
            return list;
        }

        public void InsertManualTask(string title, string desc, DateTime dueDate, byte priority, int createdBy)
        {
            string sql = @"
            INSERT INTO WorkTasks(Title, Description, DueDate, Priority, IsDone, CreatedBy)
            VALUES(@Title, @Desc, @DueDate, @Priority, 0, @CreatedBy)";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Desc", (object)(desc ?? ""));
                cmd.Parameters.AddWithValue("@DueDate", dueDate.Date);
                cmd.Parameters.AddWithValue("@Priority", priority);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ToggleDone(int taskId, bool isDone)
        {
            string sql = "UPDATE WorkTasks SET IsDone=@IsDone WHERE TaskId=@TaskId";
            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TaskId", taskId);
                cmd.Parameters.AddWithValue("@IsDone", isDone);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

}