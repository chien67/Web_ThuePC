using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models;
using ClosedXML.Excel;
using System.IO;
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
        public ActionResult ExportExcel()
        {
            var data = _bll.GetAllWithStats(); // giống Index

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhMuc");

                // Header
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Tên danh mục";
                worksheet.Cell(1, 3).Value = "Số lượng model";
                worksheet.Cell(1, 4).Value = "Số lượng thiết bị";
                worksheet.Cell(1, 5).Value = "Cập nhật";

                // Data
                int row = 2;
                foreach (var cate in data)
                {
                    worksheet.Cell(row, 1).Value = cate.Id;
                    worksheet.Cell(row, 2).Value = cate.CategoryName;
                    worksheet.Cell(row, 3).Value = cate.ModelCount;
                    worksheet.Cell(row, 4).Value = cate.TotalQuantity;
                    worksheet.Cell(row, 5).Value = cate.LastUpdated;
                    row++;
                }

                // Auto width cho đẹp
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DanhSachDanhMuc.xlsx");
                }
            }
        }
    }
}