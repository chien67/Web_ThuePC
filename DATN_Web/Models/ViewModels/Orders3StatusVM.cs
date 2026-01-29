using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.Models.Entities;

namespace DATN_Web.Models.ViewModels
{
    public class Orders3StatusVM
    {
        public List<OrderListRow> Preparing { get; set; } = new List<OrderListRow>();
        public List<OrderListRow> Active { get; set; } = new List<OrderListRow>();
        public List<OrderListRow> Finished { get; set; } = new List<OrderListRow>(); 
    }
}