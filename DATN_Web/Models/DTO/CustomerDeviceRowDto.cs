using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.DTO
{
    public class CustomerDeviceRowDto
    {
        public int Id { get; set; }               // CustomerDevices.Id
        public DateTime? DeliveryDate { get; set; }
        public string CategoryName { get; set; }
        public string ModelName { get; set; }
        public string Configuration { get; set; }
        public int Quantity { get; set; }
        public byte Status { get; set; }
    }
}