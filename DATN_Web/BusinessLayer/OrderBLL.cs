using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.Entities;

namespace DATN_Web.BusinessLayer
{
    public class OrderBLL
    {
        private readonly OrderDAL _orderDal;
        private readonly OrderDetailDAL _detailDal;

        public OrderBLL(OrderDAL orderDal, OrderDetailDAL detailDal)
        {
            _orderDal = orderDal;
            _detailDal = detailDal;
        }

        public int CreateOrder(Order order)
        {
            // Validate cơ bản
            if (order == null) return 0;
            if (order.CustomerId <= 0) return 0;
            if (!order.DeliveryDate.HasValue || !order.ReturnDate.HasValue) return 0;

            if (string.IsNullOrWhiteSpace(order.DeviceRequirement)) return 0;
            if (order.Quantity <= 0) return 0;
            if (order.UnitPrice <= 0) return 0;

            var start = order.DeliveryDate.Value.Date;
            var end = order.ReturnDate.Value.Date;
            if (end < start) return 0;

            // Tính số ngày thuê (cùng ngày = 1)
            order.RentDays = (end - start).Days + 1;

            // Set ngày tạo + status mặc định
            order.CreatedAt = DateTime.Now;
            if (order.Status == 0) order.Status = 0;

            // Insert Order
            int orderId = _orderDal.InsertOrder(order);
            return orderId; // >0 là ok
        }
        //Lấy danh sách đơn hàng của 1 id cụ thể
        public List<Order> GetOrdersOfCustomer(int customerId)
        {
            if (customerId <= 0) return new List<Order>();
            return _orderDal.GetOrdersByCustomerId(customerId);
        }
    }
}