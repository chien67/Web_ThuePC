using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATN_Web.Filters
{
    public class WarehouseOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var roleObj = httpContext.Session?["Role"];  // đổi key nếu bạn lưu khác
            if (roleObj == null) return false;

            return int.TryParse(roleObj.ToString(), out var role) && role == 3; // 3 = Quản lý kho
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/Index");
        }
    }
}