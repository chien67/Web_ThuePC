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
        DeviceCategoryBLL bll = new DeviceCategoryBLL();
        private readonly DeviceCategoryDAL _dal = new DeviceCategoryDAL();
        // GET: Warehouse
        public ActionResult Index()
        {
            var data = bll.GetAllWithStats();
            return View(data);
        }
        [HttpGet]
        public ActionResult CreateDeviceCategory()
        {
            return View();
        }
        [HttpPost]
        public bool CreateDeviceCategory(DeviceCategory category)
        {
            // 1. Kiểm tra nghiệp vụ cơ bản (client-side)
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return false;
            }

            // 2. Gọi DAL để kiểm tra sự tồn tại trong DB (nghiệp vụ DB)
            if (_dal.IsCategoryNameExist(category.CategoryName))
            {
                // Trả về false nếu trùng tên
                return false;
            }

            // 3. Chuẩn bị dữ liệu (Gán các giá trị mặc định/hệ thống)
            category.LastUpdated = DateTime.Now;
            category.ModelCount = 0;
            category.TotalQuantity = 0;

            // 4. Gọi DAL để thực hiện thao tác lưu DB
            return _dal.CreateDeviceCategory(category);
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