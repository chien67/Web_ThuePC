using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.Models.Entities;

namespace DATN_Web.Models.ViewModels
{
    public class CreateOrderVM
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
    }
}