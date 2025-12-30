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
                    return RedirectToAction("CreateCustomers");
                    //return RedirectToAction("Index");
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

            return View(vm);
        }
        public ActionResult DeleteCustomers()
        {
            return View();
        }

    }
}