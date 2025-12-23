using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class DeviceImport
    {
        public int ImportId { get; set; }
        public int ModelId { get; set; }
        public string Partner { get; set; } 
        public int ImportQuantity { get; set; }
        public byte ImportType { get; set; }
        public string Notes { get; set; }
    }
}