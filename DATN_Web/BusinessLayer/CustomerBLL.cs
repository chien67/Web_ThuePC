using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}