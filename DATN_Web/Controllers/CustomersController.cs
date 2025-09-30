using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATN_Web.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateCustomers()
        {
            return View();
        }
        public ActionResult DetailCustomers()
        {
            return View();
        }

    }
}