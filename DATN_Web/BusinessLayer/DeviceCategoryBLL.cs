using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models;

namespace DATN_Web.BusinessLayer
{
    public class DeviceCategoryBLL
    {
        DeviceCategoryDAL dal= new DeviceCategoryDAL();
        public bool CreateDeviceCategory(DeviceCategory deviceCategory)
        {
            return dal.CreateDeviceCategory(deviceCategory);
        }

    }
}