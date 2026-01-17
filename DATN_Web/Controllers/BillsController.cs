using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATN_Web.BusinessLayer;
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
                Finished = _orderBLL.GetOrdersReadyForPayment()
        .Select(o => new OrderListRow
        {
            OrderId = o.OrderId,
            DeliveryDate = o.DeliveryDate,
            DeviceRequirement = o.DeviceRequirement,
            Quantity = o.Quantity,
            RentDays = o.RentDays,
            UnitPrice = o.UnitPrice,
            DepositAmount = o.DepositAmount
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
                bill.PaidDate = DateTime.Now;
                bill.PaidUserId = Convert.ToInt32(Session["UserId"]);

                _billBLL.PayOrder(bill);

                TempData["Success"] = "Thanh toán thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(bill);
            }
        }
    }
}