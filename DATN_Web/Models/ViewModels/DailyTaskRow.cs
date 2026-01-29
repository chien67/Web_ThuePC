using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class DailyTaskRow
    {
        public string Source { get; set; } // "AUTO" hoặc "MANUAL"
        public int? TaskId { get; set; }   // manual mới có
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DeliveryDate { get; set; }  // ngày giao
        public DateTime WorkDate { get; set; }      // ngày chuẩn bị (DeliveryDate - 1)
        public byte Priority { get; set; } // 1/2/3
        public bool IsDone { get; set; }   // manual mới có
        public string Link { get; set; }   // link sang Order/chi tiết
    }
}