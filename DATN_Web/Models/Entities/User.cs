using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }  // lưu hash sha256
        public string FullName { get; set; }
        public string Email { get; set; }
        public byte Role { get; set; }            
        public bool IsActive { get; set; }
    }
}