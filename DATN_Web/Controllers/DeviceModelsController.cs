using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Models;

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
        public ActionResult Create(DeviceModel model) // Đã đổi thành ActionResult
        {
            // ... Logic kiểm tra ModelState.IsValid và gọi BLL ...

            if (_bll.CreateDeviceModel(model))
            {
                // Trả về RedirectToActionResult, kế thừa từ ActionResult
                TempData["Success"] = $"Thêm Model '{model.ModelName}' thành công. ID mới: {model.Id}";
                return RedirectToAction("Index", new { categoryId = model.CategoryId });
            }
            else
            {
                // Trả về ViewResult, kế thừa từ ActionResult
                ModelState.AddModelError("", "Lỗi nghiệp vụ: Tên Model đã tồn tại.");
                return View("Index", model);
            }
        }
        // Lưu ý: Bạn nên nhận CategoryId để biết quay lại trang nào
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

            // Gọi BLL để xóa
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
            if (modelId <= 0)
            {
                TempData["Error"] = "ID Model không hợp lệ.";
                // Chuyển hướng về trang danh mục
                return RedirectToAction("Index", "Categories");
            }

            // 1. Gọi BLL để lấy thông tin chi tiết Model
            var modelToImport = _bll.GetModelDetails(modelId);

            if (modelToImport == null)
            {
                TempData["Error"] = "Không tìm thấy Model này.";
                return RedirectToAction("Index", "Categories");
            }

            // 2. Trả về View cùng với Model (hoặc View Model/ImportEntryModel)
            // Giả sử chúng ta truyền Model gốc để hiển thị tên thiết bị
            return View(modelToImport);
        }
    }
}