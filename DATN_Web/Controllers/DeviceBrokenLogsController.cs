using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class DeviceBrokenLogsController : Controller
    {
        private readonly DeviceBrokenLogBLL _bll;

        public DeviceBrokenLogsController(DeviceBrokenLogBLL deviceBrokenLogBLL)
        {
            _bll = deviceBrokenLogBLL;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var list = _bll.GetAll();   
            return View(list);          
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Models = new SelectList(
                _bll.GetAllModels(),
                "Id",
                "ModelName"
            );

            ViewBag.Customers = new SelectList(
                _bll.GetAllCustomers(),
                "CustomerId",
                "CustomerName"
            );

            return View(new CreateBrokenDeviceVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBrokenDeviceVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Models = new SelectList(_bll.GetAll(), "Id", "ModelName");
                ViewBag.Customers = new SelectList(_bll.GetAll(), "CustomerId", "CustomerName");
                return View(vm);
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            _bll.Create(vm, userId);

            TempData["Success"] = "Đã báo hỏng thiết bị";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult UpdateCost(int id)
        {
            var log = _bll.GetById(id);
            if (log == null) return HttpNotFound();

            var vm = new UpdateRepairCostVM
            {
                BrokenLogId = log.BrokenLogId,
                ModelText = log.ModelName + " - " + log.Configuration,
                CustomerName = log.DisplayCustomer,
                Quantity = log.Quantity,
                BrokenReason = log.BrokenReason,
                EstimatedCost = log.EstimatedCost ?? 0,
                CostNote = log.CostNote
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCost(UpdateRepairCostVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {
                _bll.UpdateRepairCost(vm);

                TempData["Success"] = "Đã cập nhật giá sửa";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
    }
}