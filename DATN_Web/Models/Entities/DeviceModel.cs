using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models
{
    public class DeviceModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string ModelName { get; set; }
        public string Configuration { get; set; }
        public int TotalQuantity { get; set; }
        public int InStockQuantity { get; set; }
        public int InUseQuantity { get; set; }
        public int BrokenQuantity { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        // Navigation
        public DeviceCategory Category { get; set; }
    }
}