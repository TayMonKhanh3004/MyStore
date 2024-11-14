using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStore.Models.ViewModels
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}