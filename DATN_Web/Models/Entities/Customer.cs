using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public Byte CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string RepresentativeName { get; set; }
        public string TaxCode { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public string CustomerNote { get; set; }
    }
}