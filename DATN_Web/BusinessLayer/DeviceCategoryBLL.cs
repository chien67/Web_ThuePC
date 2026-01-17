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
        public List<DeviceCategoryVM> GetAllWithStats()
        {
            var list = dal.GetCategoryStats();

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