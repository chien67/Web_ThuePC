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
        private readonly DeviceModelBLL _service = new DeviceModelBLL();
        private DeviceModelBLL _bll = new DeviceModelBLL();
        // GET: DeviceModels
        public ActionResult Index(int categoryId)
        {
            var vm = _service.GetByCategory(categoryId);
            if (vm == null)
                return HttpNotFound();

            return View(vm);
        }
        [HttpGet]
        public ActionResult Create(int categoryId)
        {
            var vm = new ModelCreate
            {
                CategoryId = categoryId
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Create(ModelCreate model)
        {
            if (string.IsNullOrWhiteSpace(model.ModelName)
                    || string.IsNullOrWhiteSpace(model.Configuration))
            {
                // quay lại Index, không mất CategoryId
                return RedirectToAction("Index", new { categoryId = model.CategoryId });
            }

            _bll.Create(model);

            return RedirectToAction("Index", new { categoryId = model.CategoryId });
        }
    }
}