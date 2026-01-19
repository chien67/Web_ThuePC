using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;

namespace DATN_Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ReportBLL _bll = new ReportBLL();

        public ActionResult Index()
        {
            int year = DateTime.Now.Year;
            var vm = _bll.GetDashboardChart(year);
            return View(vm);
        }
    }
}