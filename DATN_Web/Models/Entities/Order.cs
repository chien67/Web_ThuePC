using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DATN_Web.Models.Enum;

namespace DATN_Web.Models.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeviceRequirement { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số ngày thuê")]
        public int RentDays { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int Quantity { get; set; }
        public string DeliveryAddress { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
        public decimal UnitPrice{ get; set; }
        public decimal DepositAmount { get; set; }
        public OrderStatus Status { get; set; }
        public byte CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string RepresentativeName { get; set; }
    }
}