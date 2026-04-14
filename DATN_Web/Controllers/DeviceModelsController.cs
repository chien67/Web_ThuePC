using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Models;
using DATN_Web.Models.Entities;
using DATN_Web.Models.ViewModels;
using ClosedXML.Excel;
using System.IO;

namespace DATN_Web.Controllers
{
    public class DeviceModelsController : Controller
    {

        private readonly DeviceModelBLL _bll;
        public DeviceModelsController(DeviceModelBLL bll)
        {
            // Gán đối tượng BLL được tiêm vào biến _bll
            _bll = bll;
        }
        // GET: DeviceModels
        [HttpGet]
        public ActionResult Index(int categoryId)
        {
            var vm = _bll.GetByCategory(categoryId);
            if (vm == null)
                return HttpNotFound();

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeviceModel model)
        {
            if (_bll.CreateDeviceModel(model))
            {
                TempData["Success"] = $"Thêm Model '{model.ModelName}' thành công. ID mới: {model.Id}";
                return RedirectToAction("Index", new { categoryId = model.CategoryId });
            }
            else
            {
                ModelState.AddModelError("", "Lỗi nghiệp vụ: Tên Model đã tồn tại.");
                return View("Index", model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Nên có để bảo mật
        public ActionResult DeleteModel(int modelId, int categoryId)
        {
            if (modelId <= 0)
            {
                TempData["Error"] = "ID Model không hợp lệ.";
                // Chuyển hướng về trang danh sách (Index)
                return RedirectToAction("Index", new { categoryId = categoryId });
            }
            bool success = _bll.DeleteDeviceModel(modelId);
            if (success)
            {
                TempData["Success"] = $"Xóa Model ID: {modelId} thành công.";
            }
            else
            {
                // Có thể là do lỗi nghiệp vụ (Model đang được sử dụng) hoặc lỗi DB
                TempData["Error"] = "Xóa thất bại. Model có thể đang được sử dụng hoặc ID không tồn tại.";
            }

            // Luôn chuyển hướng người dùng về trang danh sách sau khi xóa (Post-Redirect-Get)
            return RedirectToAction("Index", new { categoryId = categoryId });
        }
        [HttpGet]
        public ActionResult Import(int modelId)
        {
            var deviceModel = _bll.GetModelDetails(modelId);
            if (deviceModel == null) return HttpNotFound();

            var vm = new DeviceImportVM
            {
                Import = new DeviceImport
                {
                    ModelId = deviceModel.Id,
                    ImportType = 1 // default: mua mới
                },
                DeviceModelName = deviceModel.ModelName,
                DeviceModelConfig = deviceModel.Configuration,
                CategoryId = deviceModel.CategoryId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveImport(DeviceImportVM vm)
        {
            var import = vm.Import;

            if (import.ModelId <= 0 || import.ImportQuantity <= 0)
            {
                TempData["Error"] = "Vui lòng nhập số lượng hợp lệ.";
                return RedirectToAction("Import", new { id = import.ModelId });
            }

            bool success = _bll.UpdateStock(import.ModelId, import.ImportQuantity, import.Partner, import.ImportType, import.Note);

            if (success)
            {
                TempData["Success"] = $"Nhập kho thành công {import.ImportQuantity} thiết bị.";
                var deviceModel = _bll.GetModelDetails(import.ModelId);
                return RedirectToAction("Index", "DeviceModels", new { categoryId = deviceModel.CategoryId });
            }

            TempData["Error"] = "Lỗi hệ thống hoặc nghiệp vụ khi nhập kho.";
            return RedirectToAction("Import", new { id = import.ModelId });
        }
        public ActionResult ExportExcel(int categoryId)
        {
            var vm = _bll.GetByCategory(categoryId);
            if (vm == null)
                return HttpNotFound();

            var data = vm.Models;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Models");

                // Header
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Tên Model";
                worksheet.Cell(1, 3).Value = "Cấu hình";
                worksheet.Cell(1, 4).Value = "SL Tổng";
                worksheet.Cell(1, 5).Value = "Tồn kho";
                worksheet.Cell(1, 6).Value = "Sử dụng";
                worksheet.Cell(1, 7).Value = "Hỏng";

                // Data
                int row = 2;
                foreach (var m in data)
                {
                    worksheet.Cell(row, 1).Value = m.Id;
                    worksheet.Cell(row, 2).Value = m.ModelName;
                    worksheet.Cell(row, 3).Value = m.Configuration;
                    worksheet.Cell(row, 4).Value = m.TotalQuantity;
                    worksheet.Cell(row, 5).Value = m.InStockQuantity;
                    worksheet.Cell(row, 6).Value = m.InUseQuantity;
                    worksheet.Cell(row, 7).Value = m.BrokenQuantity;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DanhSachModel.xlsx");
                }
            }
        }
    }
}