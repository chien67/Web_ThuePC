using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Models;

namespace DATN_Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerBLL _bll;
        // GET: Customer
        public CustomersController(CustomerBLL bll)
        {
            _bll = bll;
        }
        public ActionResult Index()
        {
            var data = _bll.GetListCustomer();
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
                bool result = _bll.CreateCustomers(createCus);
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
            var customer = _bll.GetCustomerDetail(id);
            if (customer == null)
                return HttpNotFound();

            return View(customer);
        }
        public ActionResult DeleteCustomers()
        {
            return View();
        }

    }
}