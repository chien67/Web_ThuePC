using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class DeviceImportVM
    {
        public DATN_Web.Models.Entities.DeviceImport Import { get; set; }

        // Chỉ để hiển thị từ bảng DeviceModel
        public string DeviceModelName { get; set; }
        public string DeviceModelConfig { get; set; }

        // nếu cần quay lại danh sách theo category
        public int CategoryId { get; set; }

    }
}