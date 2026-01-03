using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using DATN_Web.Models.Entities;

namespace DATN_Web.DataAccesLayer
{
    public class UserDAL
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }
        public User GetByUsername(string username)
        {
            const string sql = @"SELECT TOP 1 UserId, Username, PasswordHash, FullName, Email, Role, IsActive
                                 FROM Users
                                 WHERE Username = @Username";

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;
                conn.Open();

                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;

                    return new User
                    {
                        UserId = Convert.ToInt32(rd["UserId"]),
                        Username = rd["Username"].ToString(),
                        PasswordHash = rd["PasswordHash"].ToString(),
                        FullName = rd["FullName"] == DBNull.Value ? null : rd["FullName"].ToString(),
                        Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString(),
                        Role = Convert.ToByte(rd["Role"]),
                        IsActive = Convert.ToBoolean(rd["IsActive"]),
                    };
                }
            }
        }
        public int InsertUser(User u)
        {
            const string sql = @"INSERT INTO Users (Username, PasswordHash, FullName, Email, [Role], IsActive)
                                 VALUES (@Username, @PasswordHash, @FullName, @Email, @Role, @IsActive);";
            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = u.Username;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar, 256).Value = u.PasswordHash;

                cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value =
                    (object)u.FullName ?? DBNull.Value;

                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value =
                    (object)u.Email ?? DBNull.Value;

                cmd.Parameters.Add("@Role", SqlDbType.TinyInt).Value = u.Role;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = u.IsActive;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public List<User> GetAllUsers()
        {
            const string sql = @"SELECT UserId, Username, PasswordHash, FullName, Email, Role, IsActive
                         FROM Users
                         ORDER BY Username";

            var list = new List<User>();

            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new User
                        {
                            UserId = Convert.ToInt32(rd["UserId"]),
                            Username = rd["Username"].ToString(),
                            PasswordHash = rd["PasswordHash"].ToString(),
                            FullName = rd["FullName"] == DBNull.Value ? null : rd["FullName"].ToString(),
                            Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString(),
                            Role = Convert.ToByte(rd["Role"]),
                            IsActive = Convert.ToBoolean(rd["IsActive"]),
                        });
                    }
                }
            }

            return list;
        }
    }
}