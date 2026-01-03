using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Antlr.Runtime.Tree;
using DATN_Web.DataAccesLayer;
using DATN_Web.Models;

namespace DATN_Web.BusinessLayer
{

    public class CustomerBLL
    {
        private readonly CustomerDAL _customerDAL;

        public CustomerBLL(CustomerDAL customerDal)
        {
            _customerDAL = customerDal;
        }

        public List<Customer> GetListCustomer()
        {
            return _customerDAL.GetAll();
        }
        public bool CreateCustomers(Customer c)
        {
           return _customerDAL.CreateCustomers(c);
        }
        public Customer GetCustomerDetail(int id)
        {
            if (id <= 0) return null;
            return _customerDAL.GetById(id);
        }
        public Customer GetById(int id)
        {
            if (id <= 0) return null;
            return (_customerDAL.GetById(id));
        }
        public int Update(Customer c)
        {
            // Có thể thêm validate nghiệp vụ tại đây nếu muốn
            return _customerDAL.UpdateCustomer(c);
        }
        public bool DeleteCustomer(int customerId)
        {
            if (customerId <= 0) return false;
            var order = _customerDAL.DeleteCustomer(customerId);
            return _customerDAL.DeleteCustomer(customerId);
        }
    }
}