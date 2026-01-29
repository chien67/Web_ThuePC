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
            ViewBag.Models = new SelectList(_bll.GetAllModels(),"Id","ModelName");
            ViewBag.Customers = BuildCustomerSelectList();
            return View(new CreateBrokenDeviceVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBrokenDeviceVM vm)
        {
            vm.BrokenReason = vm.BrokenReason?.Trim();

            if (string.IsNullOrWhiteSpace(vm.BrokenReason))
            {
                TempData["ToastError"] = "Vui lòng nhập lý do hỏng thiết bị";
                return RedirectToAction("Create");
            }

            // nạp dropdown (để view không lỗi nếu quay lại)
            ViewBag.Models = new SelectList(_bll.GetAllModels(), "Id", "ModelName", vm.ModelId);
            ViewBag.Customers = new SelectList(_bll.GetAllCustomers(), "CustomerId", "CustomerName", vm.CustomerId);

            if (!ModelState.IsValid)
                return View(vm);

            if (Session["UserId"] == null)
            {
                TempData["ToastError"] = "Phiên đăng nhập đã hết hạn";
                return RedirectToAction("Create");
            }

            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                _bll.Create(vm, userId);

                TempData["ToastSuccess"] = "Đã báo hỏng thiết bị";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ToastError"] = "Lỗi khi lưu: " + ex.Message;
                return RedirectToAction("Create");
            }
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

                TempData["ToastSuccess"] = "Đã cập nhật giá sửa";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Recover(int id)
        {
            if (Session["UserId"] == null)
            {
                TempData["ToastError"] = "Phiên đăng nhập đã hết hạn";
                return RedirectToAction("Index");
            }

            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                _bll.Recover(id, userId);

                TempData["ToastSuccess"] = "Đã thu hồi báo hỏng (đã sửa xong)";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ToastError"] = "Thu hồi thất bại: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        private SelectList BuildCustomerSelectList(int? selectedId = null)
        {
            var customers = _bll.GetAllCustomers()
                .Select(c => new
                {
                    c.CustomerId,
                    DisplayName =
                        c.CustomerType == 1
                            ? c.RepresentativeName
                            : (!string.IsNullOrWhiteSpace(c.RepresentativeName)
                                ? $"{c.CustomerName} ({c.RepresentativeName})"
                                : c.CustomerName)
                })
                .ToList();

            return new SelectList(customers, "CustomerId", "DisplayName", selectedId);
        }

    }
}