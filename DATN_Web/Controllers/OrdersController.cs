using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.Entities;
using DATN_Web.Models.Enum;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CustomerBLL _customerBll;
        private readonly OrderBLL _orderBll;
        private readonly CustomerDeviceBLL _cusDeviceBll;

        public OrdersController(CustomerBLL customerBll, OrderBLL orderBll, CustomerDeviceBLL cusDeviceBll)
        {
            _customerBll = customerBll;
            _orderBll = orderBll;
            _cusDeviceBll = cusDeviceBll;
        }
        private void LoadCategories()
        {
            ViewBag.Categories = _cusDeviceBll.GetCategories()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }
        // GET: Orders
        public ActionResult Index()
        {
            var vm = _orderBll.GetOrders3Tables();
            return View(vm);
        }
        [HttpGet]
        public ActionResult CreateOrder(int customerId)
        {
            var cus = _customerBll.GetCustomerDetail(customerId);
            if (cus == null) return HttpNotFound();
            // 1) Lấy danh mục để đổ dropdown
            var categories = _cusDeviceBll.GetCategories(); 

            ViewBag.Categories = categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            var vm = new CreateOrderVM
            {
                Customer = cus,
                Order = new Order
                {
                    CustomerId = customerId,
                    DeliveryDate = DateTime.Today,
                    ReturnDate = DateTime.Today,
                    DeliveryAddress = cus.Address
                }
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(CreateOrderVM vm)
        {
            LoadCategories();

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
            // 🔴 BƯỚC CHECK TỒN KHO THEO NGÀY TRÙNG
            if (!vm.Order.CategoryId.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn danh mục thiết bị.";
                return View(vm);
            }
            string error;
            bool canCreate = _orderBll.CanCreateOrder(
                vm.Order.CategoryId.Value,
                vm.Order.Quantity,
                vm.Order.DeliveryDate.Value,
                vm.Order.ReturnDate.Value,
                out error
            );

            if (!canCreate)
            {
                ModelState.AddModelError("", error);
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
            var order = _orderBll.GetOrder(id);
            if (order == null) return HttpNotFound();

            var cus = _customerBll.GetCustomerDetail(order.Order.CustomerId);
            if (cus == null) return HttpNotFound();

            // Lấy danh sách danh mục để đổ dropdown
            var categories = _cusDeviceBll.GetCategories(); // hoặc BLL bạn đang dùng

            ViewBag.Categories = categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = (order.Order.CategoryId == x.Id) // ⭐ chọn đúng danh mục hiện tại
            }).ToList();
            var vm = new CreateOrderVM
            {
                Customer = cus,
                Order = order.Order
            };
            return View("Edit", vm);
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

                return RedirectToAction(
                    "DetailCustomers",
                    "Customers",
                    new { id = vm.Order.CustomerId }
                );
            }
            TempData["Error"] = "Cập nhật đơn hàng thất bại.";

            // nạp lại customer để view không null khi lỗi
            vm.Customer = _customerBll.GetCustomerDetail(vm.Order.CustomerId);
            return RedirectToAction("DetailCustomers","Customers",new { id = vm.Order.CustomerId, tab = "orders" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrder(CreateOrderVM vm)
        {
            int orderId = vm?.Order?.OrderId ?? 0;
            int customerId = vm?.Order?.CustomerId ?? 0;

            bool ok = _orderBll.DeleteOrder(orderId);

            TempData[ok ? "Success" : "Error"] = ok ? "Đã xoá đơn hàng." : "Xoá thất bại.";
            return RedirectToAction("DetailCustomers", "Customers", new { id = customerId, tab = "orders" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishOrder(CreateOrderVM vm)
        {
            int orderId = vm?.Order?.OrderId ?? 0;
            int customerId = vm?.Order?.CustomerId ?? 0;

            if (orderId <= 0)
            {
                TempData["ToastError"] = "Đơn hàng không hợp lệ.";
                return RedirectToAction("DetailCustomers", "Customers", new { id = customerId, tab = "orders" });
            }

            // LẤY TRẠNG THÁI HIỆN TẠI TỪ DB (đừng tin dữ liệu từ form)
            var order = _orderBll.GetById(orderId);
            if (order == null)
            {
                TempData["ToastError"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DetailCustomers", "Customers", new { id = customerId, tab = "orders" });
            }

            //  Đang chuẩn bị => không cho kết thúc
            if (order.Status == OrderStatus.Preparing)
            {
                TempData["ToastError"] = "Đơn hàng đang chuẩn bị, chưa thể kết thúc.";
                return RedirectToAction("DetailCustomers", "Customers", new { id = customerId, tab = "orders" });
            }

            bool ok = _orderBll.FinishOrder(orderId);

            TempData[ok ? "ToastSuccess" : "ToastError"] =
                ok ? "Đã kết thúc đơn hàng." : "Kết thúc đơn hàng thất bại.";

            return RedirectToAction("DetailCustomers", "Customers", new { id = customerId, tab = "orders" });
        }
    }
}