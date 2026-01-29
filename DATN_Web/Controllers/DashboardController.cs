using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Filters;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ReportBLL _bll = new ReportBLL();

        [ManagerOnly]
        public ActionResult Index()
        {
            // mặc định 7 ngày gần nhất
            var vm = new ReportFilterVM
            {
                FromDate = DateTime.Today.AddDays(-6),
                ToDate = DateTime.Today
            };

            // load luôn
            try
            {
                vm = _bll.GetReport(vm.FromDate, vm.ToDate);
            }
            catch { }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ManagerOnly]
        public ActionResult Index(ReportFilterVM model)
        {
            try
            {
                var vm = _bll.GetReport(model.FromDate, model.ToDate);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }
    }
}