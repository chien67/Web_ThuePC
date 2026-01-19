using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Models;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerBLL _customerBll;
        private readonly OrderBLL _orderBLL;
        // GET: Customer
        public CustomersController(CustomerBLL customerBll, OrderBLL orderBLL)
        {
            _customerBll = customerBll;
            _orderBLL = orderBLL;
        }
        public ActionResult Index()
        {
            var data = _customerBll.GetListCustomer();
            return View(data);
        }
        [HttpGet]
        public ActionResult CreateCustomers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCustomers(Customer createCus)
        {
            if (ModelState.IsValid)
            {
                bool result = _customerBll.CreateCustomers(createCus);
                if (result)
                {
                    TempData["msg"] = "Thêm mới khách hàng thành công";
                    return RedirectToAction("Index");
                }
                TempData["msg"] = "Lỗi: Thêm khách hàng thất bại";

            }
            return View(createCus);
        }
        [HttpGet]
        public ActionResult DetailCustomers(int id)
        {
            var customer = _customerBll.GetCustomerDetail(id);
            if (customer == null)
                return HttpNotFound();
            var vm = new CustomerDetailVM
            {
                Customer = customer,
                Orders = _orderBLL.GetOrdersOfCustomer(id)
            };
            var deviceBll = new CustomerDeviceBLL();
            vm.CustomerDevices = deviceBll.GetDevicesByCustomerId(id, onlyInUse: true);
            vm.ReturnHistories = deviceBll.GetReturnHistoryByCustomerId(id);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCustomers(int id)
        {
            _customerBll.DeleteCustomer(id);
            return RedirectToAction("Index");
        }

        // GET: Customers/Edit/5
        public ActionResult EditCustomers(int id)
        {
            var customer = _customerBll.GetById(id);
            if (customer == null) return HttpNotFound();

            return View(customer); // đổ dữ liệu lên form
        }

        // POST: Customers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomers(Customer model)
        {
            // Cá nhân (1) → cần tên người (RepresentativeName)
            if (model.CustomerType == 1 && string.IsNullOrWhiteSpace(model.RepresentativeName))
            {
                ModelState.AddModelError("RepresentativeName", "Cá nhân cần họ tên.");
            }

            // Doanh nghiệp (2) → cần tên công ty (CustomerName)
            if (model.CustomerType == 2 && string.IsNullOrWhiteSpace(model.CustomerName))
            {
                ModelState.AddModelError("CustomerName", "Doanh nghiệp cần tên công ty.");
            }

            if (!ModelState.IsValid)
                return View(model);
            if (model.CustomerType == 1)
                model.CustomerName = null;
            _customerBll.Update(model);
            return RedirectToAction("Index"); // hoặc quay về list khách hàng
        }
    }
}