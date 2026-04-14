using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.Filters;

namespace DATN_Web.Controllers
{
    public class WorkTaskController : Controller
    {
        // GET: WorkTask
        [HttpGet]
        public ActionResult LoginCode()
        {
            return View();
        }
    }
}