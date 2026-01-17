using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models.DTO;
using DATN_Web.Models.Entities;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.BusinessLayer
{
    public class CustomerDeviceBLL
    {
        private readonly CustomerDeviceDAL _dal = new CustomerDeviceDAL();

        // 1) Danh mục
        public List<(int Id, string Name)> GetCategories()
        {
            return _dal.GetCategories();
        }
        public List<User> GetDeliveryUsers()
        {
            return _dal.GetDeliveryUsers();
        }
        // 2) Model theo danh mục (Ajax)
        public List<DeviceModelOptionDto> GetModelsByCategory(int categoryId)
        {
            if (categoryId <= 0) return new List<DeviceModelOptionDto>();
            return _dal.GetModelsByCategory(categoryId);
        }

        // 3) Xuất thiết bị cho khách
        public void AssignDeviceToCustomer(int customerId, int modelId, int quantity, int deliveryUserId)
        {
            // DAL đã có check tồn kho + transaction
            _dal.AssignDeviceToCustomer(customerId, modelId, quantity, deliveryUserId);
        }
        public List<CustomerDeviceRowDto> GetDevicesByCustomerId(int customerId, bool onlyInUse = true)
        {
            if (customerId <= 0) return new List<CustomerDeviceRowDto>();
            return _dal.GetDevicesByCustomerId(customerId, onlyInUse);
        }
        public ReturnDeviceVM GetReturnFormData(int customerDeviceId)
        {
            return _dal.GetReturnFormData(customerDeviceId);
        }
        public void ReturnDevicePartial(int customerDeviceId, int returnQty)
        {
            _dal.ReturnDevicePartial(customerDeviceId, returnQty);
        }
        public List<CustomerDeviceRowDto> GetReturnHistoryByCustomerId(int customerId)
        {
            if (customerId <= 0) return new List<CustomerDeviceRowDto>();
            return _dal.GetReturnHistoryByCustomerId(customerId);
        }
        // Lấy danh sách thiết bị chờ nhận của nhân viên
        public List<CustomerDevice> GetWaitingReceive(int deliveryUserId)
        {
            if (deliveryUserId <= 0)
                throw new Exception("Người dùng không hợp lệ.");

            return _dal.GetWaitingReceiveByDeliveryUser(deliveryUserId);
        }

        // Nhân viên xác nhận đã nhận & giao cho khách
        public void Confirm(int customerDeviceId, int deliveryUserId)
        {
            if (customerDeviceId <= 0)
                throw new Exception("Thiết bị không hợp lệ.");

            if (deliveryUserId <= 0)
                throw new Exception("Người dùng không hợp lệ.");
            _dal.ConfirmByDeliveryUser(customerDeviceId, deliveryUserId);
        }
    }
}