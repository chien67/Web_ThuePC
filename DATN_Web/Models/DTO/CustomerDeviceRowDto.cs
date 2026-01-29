using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.Models.Enum;

namespace DATN_Web.Models.DTO
{
    public class CustomerDeviceRowDto
    {
        public int Id { get; set; }               
        public DateTime? DeliveryDate { get; set; }
        public string CategoryName { get; set; }
        public string ModelName { get; set; }
        public string Configuration { get; set; }
        public int Quantity { get; set; }
        public CustomerDeviceStatus Status { get; set; }
        public int? DeliveryUserId { get; set; }
    }
}