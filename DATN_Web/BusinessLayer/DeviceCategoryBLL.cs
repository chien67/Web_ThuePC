using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.BusinessLayer
{
    public class DeviceCategoryBLL
    {
        DeviceCategoryDAL dal= new DeviceCategoryDAL();

        public List<DeviceCategory> GetAll()
        {
            return dal.GetAllCategory();
        }
        public bool CreateDeviceCategory(DeviceCategory deviceCategory)
        {
            return dal.CreateDeviceCategory(deviceCategory);
        }
        // Trang Index danh mục (có thống kê)
        public List<DeviceCategoryVM> GetAllWithStats()
        {
            var list = dal.GetCategoryStats();

            // (Tuỳ chọn) xử lý nghiệp vụ nhẹ ở đây nếu cần:
            // ví dụ đảm bảo không âm 
            foreach (var x in list)
            {
                if (x.ModelCount < 0) x.ModelCount = 0;
                if (x.TotalQuantity < 0) x.TotalQuantity = 0;
            }

            return list;
        }
    }
}