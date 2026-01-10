using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATN_Web.Models.ViewModels
{
    public class ImportDeviceVM
    {
        public int CustomerId { get; set; }
        public int? CategoryId { get; set; }
        public int? ModelId { get; set; }
        public int InStockQuantity { get; set; }
        public int Quantity { get; set; }
        //controller đổ dropdown danh mục
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}