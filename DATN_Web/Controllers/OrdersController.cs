using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.Models.Entities;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CustomerBLL _customerBll;
        private readonly OrderBLL _orderBll;

        public OrdersController(CustomerBLL customerBll, OrderBLL orderBll)
        {
            _customerBll = customerBll;
            _orderBll = orderBll;
        }
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateOrder(int customerId)
        {
            var cus = _customerBll.GetCustomerDetail(customerId);
            if (cus == null) return HttpNotFound();

            var vm = new CreateOrderVM
            {
                Customer = cus,
                Order = new Order
                {
                    CustomerId = customerId,
                    DeliveryDate = DateTime.Today,
                    ReturnDate = DateTime.Today,
                    DeliveryAddress = cus.Address // auto fill nếu muốn
                }
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(CreateOrderVM vm)
        {
            if (vm == null || vm.Order == null)
            {
                TempData["Error"] = "Đơn hàng không hợp lệ.";
                return View(vm);
            }

            // nạp lại customer để View không null
            if (vm.Order.CustomerId > 0)
                vm.Customer = _customerBll.GetCustomerDetail(vm.Order.CustomerId);

            // (Optional) validate nhanh ở controller để báo message rõ hơn
            if (vm.Order.CustomerId <= 0 ||
                !vm.Order.DeliveryDate.HasValue ||
                !vm.Order.ReturnDate.HasValue ||
                string.IsNullOrWhiteSpace(vm.Order.DeviceRequirement) ||
                vm.Order.Quantity <= 0 ||
                vm.Order.UnitPrice <= 0)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ: ngày giao/trả, yêu cầu thiết bị, số lượng, đơn giá.";
                return View(vm);
            }

            int orderId = _orderBll.CreateOrder(vm.Order);

            if (orderId > 0)
            {
                TempData["Success"] = "Tạo đơn hàng thành công.";
                return RedirectToAction("DetailCustomers", "Customers", new { id = vm.Order.CustomerId });
            }

            TempData["Error"] = "Tạo đơn hàng thất bại.";
            return View(vm);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var order = _orderBll.GetOrder(id); // BLL gọi DAL GetOrderById
            if (order == null) return HttpNotFound();

            var cus = _customerBll.GetCustomerDetail(order.CustomerId);

            var vm = new CreateOrderVM
            {
                Customer = cus,
                Order = order
            };
            return View("Edit", vm); // ✅ view mới, không quay create
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateOrderVM vm)
        {
            if (vm == null || vm.Order == null)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return View("EditOrder", vm);
            }

            // Update
            bool ok = _orderBll.UpdateOrder(vm.Order);

            if (ok)
            {
                TempData["Success"] = "Cập nhật đơn hàng thành công.";

                // ✅ QUAY VỀ TRANG DETAIL CUSTOMER
                return RedirectToAction(
                    "DetailCustomers",
                    "Customers",
                    new { id = vm.Order.CustomerId }
                );
            }
            System.Diagnostics.Debug.WriteLine(">>> POST Edit called");
            System.Diagnostics.Debug.WriteLine("OrderId = " + vm?.Order?.OrderId);
            System.Diagnostics.Debug.WriteLine("CustomerId = " + vm?.Order?.CustomerId);
            System.Diagnostics.Debug.WriteLine("UnitPrice = " + vm?.Order?.UnitPrice);
            System.Diagnostics.Debug.WriteLine("Deposit = " + vm?.Order?.DepositAmount);
            System.Diagnostics.Debug.WriteLine("Quantity = " + vm?.Order?.Quantity);
            System.Diagnostics.Debug.WriteLine("Delivery = " + vm?.Order?.DeliveryDate);
            System.Diagnostics.Debug.WriteLine("Return = " + vm?.Order?.ReturnDate);
            TempData["Error"] = "Cập nhật đơn hàng thất bại.";

            // nạp lại customer để view không null khi lỗi
            vm.Customer = _customerBll.GetCustomerDetail(vm.Order.CustomerId);
            return RedirectToAction(
    "DetailCustomers",
    "Customers",
    new { id = vm.Order.CustomerId, tab = "orders" }
);
        }
        public ActionResult DeleteOrder()
        {
            return View();
        }
    }
}