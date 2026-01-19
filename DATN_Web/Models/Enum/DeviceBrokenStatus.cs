using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Enum
{
    public enum DeviceBrokenType : byte
    {
        Broken = 1, // Hỏng
        Lost = 2    // Mất
    }

    public enum DeviceBrokenStatus : byte
    {
        New = 1,        // Mới báo
        InRepair = 2,   // Đang sửa
        Fixed = 3,      // Đã sửa xong
        Completed = 4   // Đã xử lý xong
    }
}