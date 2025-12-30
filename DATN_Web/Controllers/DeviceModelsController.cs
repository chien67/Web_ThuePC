using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Models;
using DATN_Web.Models.Entities;
using DATN_Web.Models.ViewModels;

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
    }
}