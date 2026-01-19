using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.DTO
{
    public class DeviceBrokenLogRowDto
    {
        public int BrokenLogId { get; set; }

        public string ModelName { get; set; }
        public string Configuration { get; set; }

        public int Quantity { get; set; }
        public string BrokenReason { get; set; }

        public decimal? EstimatedCost { get; set; }
        
        public string CustomerName { get; set; }
        public string CostNote { get; set; }
        public string RepresentativeName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DisplayCustomer =>
            !string.IsNullOrEmpty(CustomerName)
                ? CustomerName
                : RepresentativeName;
    }
}