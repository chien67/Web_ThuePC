using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Nhập tài khoản")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public byte Role { get; set; } // mặc định 1
    }
}