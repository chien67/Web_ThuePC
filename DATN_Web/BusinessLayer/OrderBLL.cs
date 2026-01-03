using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.Entities;
using DATN_Web.Models.Enum;
using DATN_Web.Models.ViewModels;

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
        //Lấy danh sách đơn hàng của 1 id customer cụ thể
        public List<Order> GetOrdersOfCustomer(int customerId)
        {
            if (customerId <= 0) return new List<Order>();
            var list = _orderDal.GetOrdersByCustomerId(customerId);
            var today = DateTime.Today;
            foreach (var o in list)
            {
                if (!o.DeliveryDate.HasValue) continue;

                var expireDate = o.DeliveryDate.Value.Date.AddDays(o.RentDays);

                if (today >= expireDate)
                    o.Status = 2; // Quá hạn
            }

            return list;
        }
        public Order GetOrder(int orderId)
        {
            if (orderId <= 0) return null;
            return _orderDal.GetOrderById(orderId);
        }
        public bool UpdateOrder(Order order)
        {
            if (order == null) return false;
            if (order.OrderId <= 0) return false;
            if (order.CustomerId <= 0) return false;

            if (!order.DeliveryDate.HasValue || !order.ReturnDate.HasValue) return false;
            var start = order.DeliveryDate.Value.Date;
            var end = order.ReturnDate.Value.Date;
            if (end < start) return false;

            if (string.IsNullOrWhiteSpace(order.DeviceRequirement)) return false;
            if (order.Quantity <= 0) return false;
            if (order.UnitPrice <= 0) return false;

            // tính lại số ngày thuê
            order.RentDays = (end - start).Days + 1;
            return _orderDal.UpdateOrder(order);
            //System.Diagnostics.Debug.WriteLine(">>> BLL UpdateOrder called");

            //if (order == null) { System.Diagnostics.Debug.WriteLine("Fail: order null"); return false; }
            //if (order.OrderId <= 0) { System.Diagnostics.Debug.WriteLine("Fail: OrderId <= 0"); return false; }
            //if (order.CustomerId <= 0) { System.Diagnostics.Debug.WriteLine("Fail: CustomerId <= 0"); return false; }

            //if (!order.DeliveryDate.HasValue) { System.Diagnostics.Debug.WriteLine("Fail: DeliveryDate null"); return false; }
            //if (!order.ReturnDate.HasValue) { System.Diagnostics.Debug.WriteLine("Fail: ReturnDate null"); return false; }

            //var start = order.DeliveryDate.Value.Date;
            //var end = order.ReturnDate.Value.Date;
            //if (end < start) { System.Diagnostics.Debug.WriteLine("Fail: end < start"); return false; }

            //if (string.IsNullOrWhiteSpace(order.DeviceRequirement))
            //{ System.Diagnostics.Debug.WriteLine("Fail: DeviceRequirement blank"); return false; }

            //if (order.Quantity <= 0) { System.Diagnostics.Debug.WriteLine("Fail: Quantity <= 0"); return false; }
            //if (order.UnitPrice <= 0) { System.Diagnostics.Debug.WriteLine("Fail: UnitPrice <= 0"); return false; }

            //order.RentDays = (end - start).Days + 1;

            //System.Diagnostics.Debug.WriteLine(">>> Calling DAL...");
            //return _orderDal.UpdateOrder(order);
        }
        public bool DeleteOrder(int orderId)
        {
            if (orderId <= 0) return false;
            var order = _orderDal.GetOrderById(orderId);
            if (order == null) return false;
            return _orderDal.DeleteOrder(orderId);
        }
        public bool FinishOrder(int orderId)
        {
            if (orderId <= 0) return false;

            var order = _orderDal.GetOrderById(orderId);
            if (order == null) return false;

            // chỉ cho kết thúc khi đang hoạt động
            if (order.Status != 1 && order.Status != 2) return false;


            return _orderDal.FinishOrder(orderId);
        }
        public Orders3StatusVM GetOrders3Tables()
        {
            return new Orders3StatusVM
            {
                Preparing = _orderDal.GetOrdersByStatus(OrderStatus.Preparing),
                Active = _orderDal.GetOrdersByStatus(OrderStatus.Active),
                Finished = _orderDal.GetOrdersByStatus(OrderStatus.Finished),
            };
        }
    }
}