using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class CreateBrokenDeviceVM
    {
        [Required]
        public int ModelId { get; set; }

        public int? CustomerId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        public string BrokenReason { get; set; }
    }
}