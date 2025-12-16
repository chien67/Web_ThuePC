using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATN_Web.Models.Entities
{
    public class ImportEntryModel
    {
        public int EntryId { get; set; }
        public int ModelId { get; set; }
        public string ModelName { get; set; } 
        public int ImportQuantity { get; set; }
        public string Partner { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public DateTime ImportDate { get; set; }
    }
}