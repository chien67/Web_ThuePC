using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
using DATN_Web.BusinessLogicLayer;
using DATN_Web.Filters;
using DATN_Web.Models.Entities;
using DATN_Web.Models.Enum;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class BillsController : Controller
    {
        private readonly BillBLL _billBLL;
        private readonly OrderBLL _orderBLL;
        // GET: Customer
        public BillsController(BillBLL billBLL, OrderBLL orderBLL)
        {
            _billBLL = billBLL;
            _orderBLL = orderBLL;
        }
        // Danh sách đơn hàng chưa thanh toán
        [ManagerOnly]
        public ActionResult Index()
        {
            return View(new Orders3StatusVM
            {
                Finished = _orderBLL.GetOrdersReadyForPayment().Select(o => new OrderListRow
                {
                    OrderId = o.OrderId,
                    DeliveryDate = o.DeliveryDate,
                    DeviceRequirement = o.DeviceRequirement,
                    Quantity = o.Quantity,
                    RentDays = o.RentDays,
                    UnitPrice = o.UnitPrice,
                    DepositAmount = o.DepositAmount,
                    CustomerType = o.CustomerType,
                    CustomerName = o.CustomerName,
                    RepresentativeName = o.RepresentativeName,
                    IsPaid = _billBLL.IsPaidByOrderId(o.OrderId)
                }).ToList()
            });
        }

        // GET: /Payment/Pay?orderId=1
        [ManagerOnly]
        public ActionResult Pay(int orderId)
        {
            var o = _orderBLL.GetById(orderId);

            if (o == null)
                return RedirectToAction("Index");

            // CHỐT: chỉ thanh toán khi đã xác nhận trả máy
            if (o.Status != OrderStatus.Finished)
            {
                TempData["Error"] = "Đơn hàng chưa được xác nhận trả máy.";
                return RedirectToAction("Index");
            }

            decimal rental = o.RentDays * o.UnitPrice * o.Quantity;
            decimal deposit = o.DepositAmount;

            Bill bill = new Bill
            {
                OrderId = o.OrderId,
                RentalAmount = rental,
                DepositAmount = deposit,
                TotalAmount = rental + deposit
            };

            return View(bill);
        }

        // POST: /Payment/Pay
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ManagerOnly]
        public ActionResult Pay(Bill bill)
        {
            try
            {
                if (bill == null || bill.OrderId <= 0)
                {
                    TempData["Error"] = "Dữ liệu không hợp lệ.";
                    return RedirectToAction("Index");
                }

                // ✅ chặn thanh toán lại (vì danh sách đang lấy từ Orders nên có thể bấm lại)
                var existed = _billBLL.GetByOrderId(bill.OrderId); // cần thêm hàm này trong BLL (bên dưới)
                if (existed != null && existed.Status == true)
                {
                    TempData["Error"] = "Đơn hàng này đã được thanh toán rồi.";
                    return RedirectToAction("Index");
                }

                bill.PaidDate = DateTime.Now;
                bill.PaidUserId = Convert.ToInt32(Session["UserId"]);
                bill.Status = true; // ✅ đã thanh toán

                _billBLL.PayOrder(bill);

                TempData["Success"] = "Thanh toán thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Pay", new { orderId = bill?.OrderId ?? 0 });
            }
        }
    }
}