using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.Entities;

namespace DATN_Web.BusinessLayer
{
    public class AuthBLL
    {
        private readonly UserDAL _dal = new UserDAL();

        public User Login(string username, string password)
        {
            var user = _dal.GetByUsername(username);
            if (user == null || !user.IsActive) return null;

            var hash = Sha256(password);
            return user.PasswordHash == hash ? user : null;
        }

        public string Register(string username, string password, string fullName, string email, byte role)
        {
            User exists = _dal.GetByUsername(username);
            if (exists != null)
            {
                return "Tài khoản đã tồn tại";
            }

            User u = new User();
            u.Username = username;
            u.PasswordHash = Sha256(password);
            u.FullName = fullName;
            u.Email = email;
            u.Role = role;       // 1 user, 2 admin (nếu cho admin tạo admin)
            u.IsActive = true;

            _dal.InsertUser(u);
            return null;
        }

        public static string Sha256(string raw)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
        public List<User> GetAllUsers()
        {
            return _dal.GetAllUsers();
        }
    }
}