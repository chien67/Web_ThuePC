using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class DeviceBrokenLog
    {
        public int BrokenLogId { get; set; }
        public int ModelId { get; set; }
        public int? CustomerId { get; set; }

        public int Quantity { get; set; }
        public string BrokenReason { get; set; }

        public decimal? EstimatedCost { get; set; }
        public string CostNote { get; set; }

        public byte Status { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}