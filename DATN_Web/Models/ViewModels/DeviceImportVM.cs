using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class DeviceImportVM
    {
        public DATN_Web.Models.Entities.DeviceImport Import { get; set; }
        public string DeviceModelName { get; set; }
        public string DeviceModelConfig { get; set; }
        public int CategoryId { get; set; }

    }
}