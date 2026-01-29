using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models;

namespace DATN_Web.Controllers
{
    public class WarehouseController : Controller
    {
        DeviceCategoryBLL _bll = new DeviceCategoryBLL();
        private readonly DeviceCategoryDAL _dal = new DeviceCategoryDAL();
        // GET: Warehouse
        public ActionResult Index()
        {
            var data = _bll.GetAllWithStats();
            return View(data);
        }
        [HttpGet]
        public ActionResult CreateDeviceCategory()
        {
            return View(new DeviceCategory());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDeviceCategory(DeviceCategory model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Vui lòng nhập tên danh mục");
                return View(model);
            }

            bool ok = _bll.CreateDeviceCategory(model);

            if (ok)
            {
                TempData["ToastSuccess"] = "Thêm danh mục thành công";
                return RedirectToAction("Index"); // hoặc action list danh mục của bạn
            }

            ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại");
            return View(model);
        }
        public ActionResult DeleteDeviceCategory()
        {
            return View();
        }
        public ActionResult DetaiDeviceCategory()
        {
            return View();
        }
    }
}