using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class CreateRepairOrderVM
    {
        public int ModelId { get; set; }
        public int? CustomerId { get; set; }

        public int Quantity { get; set; }
        public string BrokenReason { get; set; }

        public string ModelText { get; set; }
        public string CustomerName { get; set; }
    }
}