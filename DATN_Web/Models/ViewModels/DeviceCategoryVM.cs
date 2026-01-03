using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class DeviceCategoryVM
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        public int ModelCount { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}