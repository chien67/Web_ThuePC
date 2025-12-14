using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DATN_Web.Models
{
    public class ModelCreate
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên Model")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập cấu hình")]
        public string Configuration { get; set; }
    }
}