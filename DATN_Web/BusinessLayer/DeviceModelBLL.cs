using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models;

namespace DATN_Web.BusinessLayer
{
    public class DeviceModelBLL
    {
        private DeviceCategoryDAL _categoryDal = new DeviceCategoryDAL();
        private DeviceModelDAL _modelDal = new DeviceModelDAL();

        public DeviceModelCategory GetByCategory(int categoryId)
        {
            var category = _categoryDal.GetById(categoryId);
            if (category == null) return null;

            var models = _modelDal.GetByCategoryId(categoryId);

            return new DeviceModelCategory
            {
                CategoryId = category.Id,
                ModelName = category.CategoryName,

                TotalQuantity = models.Sum(x => x.TotalQuantity),
                InStockQuantity = models.Sum(x => x.InStockQuantity),
                InUseQuantity = models.Sum(x => x.InUseQuantity),
                BrokenQuantity = models.Sum(x => x.BrokenQuantity),

                Models = models
            };
        }
        public void Create(ModelCreate vm)
        {
            DeviceModel model = new DeviceModel
            {
                CategoryId = vm.CategoryId,
                ModelName = vm.ModelName.Trim(),
                Configuration = vm.Configuration.Trim(),

                TotalQuantity = 0,
                InStockQuantity = 0,
                InUseQuantity = 0,
                BrokenQuantity = 0,
                LastUpdatedAt = DateTime.Now
            };

            _modelDal.CreateDeviceModel(model);
        }
    }
}