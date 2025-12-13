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
        CustomerBLL bll = new CustomerBLL();
        // GET: Customer
        public ActionResult Index()
        {
            var data = bll.GetListCustomer();
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
                bool result = bll.CreateCustomers(createCus);
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
        public ActionResult DetailCustomers()
        {
            return View();
        }
        public ActionResult DeleteCustomers()
        {
            return View();
        }

    }
}