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
        private readonly DeviceModelDAL _modelDal;
        private readonly DeviceCategoryDAL _categoryDal;

        public DeviceModelBLL(DeviceModelDAL modelDal, DeviceCategoryDAL categoryDal) // DI cho DAL
        {

            _categoryDal = categoryDal;
            _modelDal = modelDal;
        }

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
        public bool CreateDeviceModel(DeviceModel model)
        {
            // 1. Kiểm tra nghiệp vụ cơ bản
            if (string.IsNullOrWhiteSpace(model.ModelName) || model.CategoryId <= 0)
            {
                return false;
            }
            // 4. Gọi DAL để lưu DB và lấy ID mới
            int newId = _modelDal.CreateDeviceModel(model);

            if (newId > 0)
            {
                // Cập nhật ID mới vào đối tượng BLL đang giữ
                model.Id = newId;
                return true;
            }

            return false; // Thêm thất bại (Lỗi DB, v.v.)
        }
        public bool DeleteDeviceModel(int modelId)
        {
            if (modelId <= 0)
            {
                // Không hợp lệ
                return false;
            }
            // Gọi DAL để xóa
            int rows = _modelDal.DeleteDeviceModel(modelId);

            // Nếu số dòng bị ảnh hưởng lớn hơn 0 (nghĩa là xóa thành công ít nhất 1 dòng)
            return rows > 0;
        }
        public bool UpdateStock(int modelId, int quantity, string partner, string jobType, string notes)
        {
            // Nếu bạn có bảng ImportEntry, bạn sẽ lưu lịch sử nhập kho ở đây
            // ...

            // Gọi DAL để cập nhật Model
            return _modelDal.UpdateQuantities(modelId, quantity);
        }
    }
}