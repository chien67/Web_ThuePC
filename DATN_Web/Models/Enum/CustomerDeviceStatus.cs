using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Enum
{
    public enum CustomerDeviceStatus
    {
        WaitingReceive = 1, // kho đã xuất – chờ nhân viên đến lấy
        ReceivedAndDelivered = 2, // nhân viên đã nhận và giao cho khách
        ReturnItem = 3 // Thu hồi
    }
}