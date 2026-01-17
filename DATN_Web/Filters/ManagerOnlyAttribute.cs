using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATN_Web.Filters
{
    public class ManagerOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var roleObj = httpContext.Session?["Role"];
            if (roleObj == null) return false;

            return int.TryParse(roleObj.ToString(), out var role)
                   && role == 2; // 1 = Quản lý
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/Index");
        }
    }
}