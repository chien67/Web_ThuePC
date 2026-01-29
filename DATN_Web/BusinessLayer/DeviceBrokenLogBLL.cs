using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models;
using DATN_Web.Models.DTO;
using DATN_Web.Models.Entities;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.BusinessLayer
{
    public class DeviceBrokenLogBLL
    {
        private readonly DeviceBrokenLogDAL _deviceBrokenLogDal;
        private readonly DeviceModelDAL _modelDal;
        private readonly CustomerDAL _customerDal; 

        public DeviceBrokenLogBLL(DeviceBrokenLogDAL deviceBrokenLog, DeviceModelDAL modelDal, CustomerDAL customerDal)
        {
            _deviceBrokenLogDal = deviceBrokenLog;
            _modelDal = modelDal;
            _customerDal = customerDal;
        }
        public List<DeviceBrokenLogRowDto> GetAll()
        {
            return _deviceBrokenLogDal.GetAll();
        }
        public void Create(CreateBrokenDeviceVM vm, int userId)
        {
            var log = new DeviceBrokenLog
            {
                ModelId = vm.ModelId,
                CustomerId = vm.CustomerId,
                Quantity = vm.Quantity,
                BrokenReason = vm.BrokenReason,
                CreatedBy = userId
            };

            _deviceBrokenLogDal.Create(log);
        }
        public List<DeviceModel> GetAllModels()
        {
            return _modelDal.GetAll();
        }

        public List<Customer> GetAllCustomers()
        {
            return _customerDal.GetAll();
        }
        public void UpdateRepairCost(UpdateRepairCostVM vm)
        {
            if (vm.EstimatedCost < 0)
                throw new Exception("Giá sửa không hợp lệ");

            _deviceBrokenLogDal.UpdateRepairCost(vm.BrokenLogId, vm.EstimatedCost, vm.CostNote);
        }
        public DeviceBrokenLogRowDto GetById(int brokenLogId)
        {
            if (brokenLogId <= 0)
                throw new Exception("Id không hợp lệ");

            return _deviceBrokenLogDal.GetById(brokenLogId);
        }
        public void Recover(int brokenLogId, int userId)
        {
            if (brokenLogId <= 0) throw new Exception("Id không hợp lệ");
            _deviceBrokenLogDal.Recover(brokenLogId, userId);
        }
    }
}