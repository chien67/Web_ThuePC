using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models
{
    public class DeviceCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int ModelCount { get; set; } = 0;
        public int TotalQuanity { get; set; }= 0;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}