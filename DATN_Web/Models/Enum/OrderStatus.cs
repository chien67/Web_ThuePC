using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Enum
{
    public enum OrderStatus: byte
    {
        Preparing = 0,   // Đang chuẩn bị
        Active    = 1,   // Đang hoạt động / đang thuê
        Overdue = 2, // Quá hạn
        Finished = 3  // Kết thúc đơn
    }
}