using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStore.Models;
using PagedList.Mvc;

namespace MyStore.Models.ViewModels
{
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public int quantity { get; set; }
        public decimal estimatedValue => Product.ProductPrice * quantity; // Tính giá trị tạm tính

        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 3;
        public PagedList.IPagedList<Product> RelatedProducts { get; set; }
        //public List<Product> RelatedProducts { get; set; }
        public PagedList.IPagedList<Product> TopProducts { get; set; }

    }
}