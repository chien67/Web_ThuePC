using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class CustomerDevicesController : Controller
    {
        private readonly CustomerDeviceBLL _bll = new CustomerDeviceBLL();
        // GET: CustomerDevices
        public ActionResult Index()
        {
            return View();
        }
        // GET: /CustomerDevices/Create?customerId=1
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

            try
            {
                if (vm.ModelId == null)
                    throw new Exception("Vui lòng chọn Model/Cấu hình.");

                _bll.AssignDeviceToCustomer(vm.CustomerId, vm.ModelId.Value, vm.Quantity);

                // Quay về chi tiết khách hàng (đổi đúng Action/Controller của bạn)
                return RedirectToAction("DetailCustomers", "Customers", new { id = vm.CustomerId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
    }
}