using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATN_Web.Filters
{
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null) return false;

            var session = httpContext.Session;
            if (session == null) return false;

            // phải đăng nhập
            if (session["UserId"] == null) return false;

            // role = 2 là admin
            if (session["Role"] == null) return false;

            byte role = (byte)session["Role"];
            return role == 2;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Nếu chưa login -> về login
            if (filterContext.HttpContext.Session == null || filterContext.HttpContext.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            // Nếu login rồi nhưng không phải admin -> trả về 403
            filterContext.Result = new HttpStatusCodeResult(403, "Bạn không có quyền truy cập");
        }
    }
}