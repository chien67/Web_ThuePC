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
        CustomerDAL dal = new CustomerDAL();

        public List<Customer> GetListCustomer()
        {
            return dal.GetAll();
        }
        public bool CreateCustomers(Customer c)
        {
           return dal.CreateCustomers(c);
        }
    }
}