using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.BusinessLayer
{
    public class DailyTaskBLL
    {
        private readonly WorkTaskDAL _taskDal;
        private readonly OrderBLL _orderBLL;
        public DailyTaskBLL(OrderBLL orderBLL)
        {
            _taskDal = new WorkTaskDAL();
            _orderBLL = orderBLL;
        }

        public List<DailyTaskRow> GetDailyTasks(DateTime selectedDate)
        {
            //  Lấy đơn sắp giao 7 ngày tới
            var upcoming = _orderBLL.GetUpcomingDeliveryOrders(7);

            //  AUTO task: ngày làm = ngày giao - 1
            var auto = upcoming
                .Where(o => o.DeliveryDate.HasValue)
                .Select(o =>
                {
                    var delivery = o.DeliveryDate.Value.Date;
                    var workDate = delivery.AddDays(-1); // ✅ ngày chuẩn bị

                    return new DailyTaskRow
                    {
                        Source = "AUTO",
                        TaskId = null,
                        Title = $"Chuẩn bị đơn #{o.OrderId} - {o.DisplayCustomerName}",
                        Description = $"Giao: {delivery:dd/MM/yyyy} | {o.Quantity} máy | {o.DeviceRequirement}",
                        DueDate = workDate,
                        Priority = 1,
                        IsDone = false,
                        Link = "/Orders/Details/" + o.OrderId
                    };
                })
                .Where(t => t.DueDate.Date == selectedDate.Date) // ✅ lọc theo ngày bạn chọn
                .ToList();

            // MANUAL task (quản lý thêm)
            var manual = _taskDal.GetManualTasksByDate(selectedDate);

            return auto
                .Concat(manual)
                .OrderBy(t => t.Source == "AUTO" ? 0 : 1)
                .ThenBy(t => t.Priority)
                .ToList();
        }

        public void AddManualTask(string title, string desc, DateTime dueDate, byte priority, int userId)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new Exception("Tiêu đề không được trống.");
            if (priority < 1 || priority > 3) priority = 2;

            _taskDal.InsertManualTask(title.Trim(), desc?.Trim(), dueDate, priority, userId);
        }

        public void SetDone(int taskId, bool isDone)
        {
            _taskDal.ToggleDone(taskId, isDone);
        }
    }
}