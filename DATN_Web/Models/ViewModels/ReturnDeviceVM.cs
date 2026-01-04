using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.ViewModels
{
    public class ReturnDeviceVM
    {
        public int CustomerId { get; set; }
        public int CustomerDeviceId { get; set; } // dòng đang dùng (Status=1)

        public string CategoryName { get; set; }
        public string ModelText { get; set; } // Model + cấu hình hiển thị

        public int InUseQuantity { get; set; }  // số lượng đang dùng
        public int ReturnQuantity { get; set; } // số lượng muốn thu hồi
    }
}