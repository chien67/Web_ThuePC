using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DATN_Web.Models.DTO;
using DATN_Web.Models.Entities;

namespace DATN_Web.Models.ViewModels
{
    public class CustomerDetailVM
    {
        public Customer Customer { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<CustomerDeviceRowDto> CustomerDevices { get; set; } = new List<CustomerDeviceRowDto>();
        // View gọi Model.ReturnHistories
        public List<CustomerDeviceRowDto> ReturnHistories { get; set; } = new List<CustomerDeviceRowDto>();
    }
}