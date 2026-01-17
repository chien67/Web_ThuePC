using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.Entities;

namespace DATN_Web.BusinessLayer
{
    public class BillBLL
    {
        private readonly BillDAL _billDal;
        public BillBLL()
        {
            _billDal = new BillDAL();
        }
        // Thanh toán đơn hàng
        public void PayOrder(Bill bill)
        {
            // 1️⃣ Kiểm tra đã có bill cho order này chưa
            Bill existingBill = _billDal.GetByOrderId(bill.OrderId);
            if (existingBill != null)
            {
                throw new Exception("Đơn hàng này đã được thanh toán.");
            }

            // 2️⃣ Kiểm tra dữ liệu cơ bản
            if (bill.TotalAmount <= 0)
            {
                throw new Exception("Số tiền thanh toán không hợp lệ.");
            }

            // 3️⃣ Thêm bill
            _billDal.Insert(bill);

            // 4️⃣ (ĐỂ SAU) cập nhật Order.IsPaid
        }
    }
}