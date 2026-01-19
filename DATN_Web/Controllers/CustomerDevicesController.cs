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
    public class CustomerDevicesController : Controller
    {
        private readonly CustomerDeviceBLL _bll = new CustomerDeviceBLL();
        // GET: /CustomerDevices/Create?customerId=1
        [WarehouseOnly]
        public ActionResult Create(int customerId)
        {
            var vm = new ImportDeviceVM
            {
                CustomerId = customerId,
                Categories = _bll.GetCategories()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToList(),
                DeliveryUsers = _bll.GetDeliveryUsers()
                     .Select(u => new SelectListItem
                     {
                         Value = u.UserId.ToString(),
                         Text = u.FullName
                     })
                     .ToList()
            };

            return View(vm);
        }

        // GET (Ajax): /CustomerDevices/GetModels?categoryId=1
        public JsonResult GetModels(int categoryId)
        {
            var data = _bll.GetModelsByCategory(categoryId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: /CustomerDevices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [WarehouseOnly]
        public ActionResult Create(ImportDeviceVM vm)
        {
            // Reload dropdown để khi lỗi vẫn hiện lại danh mục
            vm.Categories = _bll.GetCategories()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
            vm.DeliveryUsers = _bll.GetDeliveryUsers()
                 .Select(u => new SelectListItem
                 {
                     Value = u.UserId.ToString(),
                     Text = u.FullName
                 })
                 .ToList();
            try
            {
                if (vm.ModelId == null)
                    throw new Exception("Vui lòng chọn Model/Cấu hình.");
                if (vm.DeliveryUserId <= 0)
                    throw new Exception("Vui lòng chọn nhân viên nhận máy.");

                _bll.AssignDeviceToCustomer(vm.CustomerId, vm.ModelId.Value, vm.Quantity, vm.DeliveryUserId);

                // Quay về chi tiết khách hàng (đổi đúng Action/Controller của bạn)
                return RedirectToAction("DetailCustomers", "Customers", new { id = vm.CustomerId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
        
        [HttpGet]
        public ActionResult Return(int id)
        {
            var vm = _bll.GetReturnFormData(id);
            if (vm == null) return HttpNotFound();

            return View(vm); // Views/CustomerDevices/Return.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WarehouseOnly]
        public ActionResult Return(ReturnDeviceVM vm)
        {
            try
            {
                _bll.ReturnDevicePartial(vm.CustomerDeviceId, vm.ReturnQuantity);
                return RedirectToAction("DetailCustomers", "Customers", new { id = vm.CustomerId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // reload data display nếu cần
                var reload = _bll.GetReturnFormData(vm.CustomerDeviceId);
                if (reload != null)
                {
                    vm.CategoryName = reload.CategoryName;
                    vm.ModelText = reload.ModelText;
                    vm.InUseQuantity = reload.InUseQuantity;
                }
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(int id)
        {
            // LẤY USER ĐANG LOGIN
            int deliveryUserId = Convert.ToInt32(Session["UserId"]);

            // GỌI BLL → DAL → UPDATE Status = 2
            _bll.Confirm(id, deliveryUserId);

            // QUAY LẠI TRANG TRƯỚC (detail customer)
            return Redirect(Request.UrlReferrer?.ToString());
        }
    }
}