using System;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.Entities;

namespace DATN_Web.BusinessLogicLayer
{
    public class BillBLL
    {
        private readonly BillDAL _billDal;

        public BillBLL()
        {
            _billDal = new BillDAL();
        }

        // Thanh toán: bill PHẢI tồn tại trước (được tạo khi kết thúc đơn), Status=0
        // Khi bấm thanh toán -> update Status=1
        public void PayOrder(Bill bill)
        {
            if (bill == null) throw new Exception("Dữ liệu hóa đơn không hợp lệ.");
            if (bill.OrderId <= 0) throw new Exception("OrderId không hợp lệ.");
            if (bill.TotalAmount <= 0) throw new Exception("Số tiền thanh toán không hợp lệ.");
            if (bill.PaidUserId <= 0) throw new Exception("Không xác định người thanh toán.");

            var existing = _billDal.GetByOrderId(bill.OrderId);

            // 1) đã thanh toán rồi
            if (existing != null && existing.Status == true)
                throw new Exception("Đơn hàng này đã được thanh toán.");

            // 2) có bill nhưng chưa thanh toán (Status=0) -> update
            if (existing != null && existing.Status == false)
            {
                bool ok = _billDal.Pay(existing.BillId, bill.PaidUserId);
                if (!ok) throw new Exception("Thanh toán thất bại. Vui lòng thử lại.");
                return;
            }

            // 3) chưa có bill -> insert bill đã thanh toán (Status=1)
            bill.Status = true;
            if (bill.PaidDate == DateTime.MinValue) bill.PaidDate = DateTime.Now;
            bill.CreatedDate = DateTime.Now;

            _billDal.InsertPaid(bill);
        }
        public Bill GetByOrderId(int orderId)
        {
            return _billDal.GetByOrderId(orderId);
        }
        public bool IsPaidByOrderId(int orderId)
        {
            return _billDal.IsPaidByOrderId(orderId);
        }
    }
}
