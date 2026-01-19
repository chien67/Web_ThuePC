using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class UpdateRepairCostVM
    {
        public int BrokenLogId { get; set; }

        public string ModelText { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public string BrokenReason { get; set; }

        [Required]
        [Range(0, 100000000)]
        public decimal EstimatedCost { get; set; }

        public string CostNote { get; set; }
    }
}