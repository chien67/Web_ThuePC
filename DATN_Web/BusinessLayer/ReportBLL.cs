using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.BusinessLayer
{
    public class ReportBLL
    {
        private readonly ReportDAL _dal;

        public ReportBLL()
        {
            _dal = new ReportDAL();
        }

        public ReportFilterVM GetReport(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
                throw new Exception("Vui lòng chọn đủ 2 mốc thời gian.");

            DateTime from = fromDate.Value.Date;
            DateTime to = toDate.Value.Date.AddDays(1).AddTicks(-1); // inclusive cuối ngày

            if (from > to)
                throw new Exception("Từ ngày không được lớn hơn Đến ngày.");

            var data = _dal.GetRevenueAndNewCustomers(from, to);
            data.FromDate = fromDate;
            data.ToDate = toDate;
            return data;
        }
    }
}