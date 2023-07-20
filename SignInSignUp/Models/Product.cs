using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignInSignUp.Models
{
    public class ProductsTable 
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductCategory { get; set; }
        public bool IsActive { get; set; }

    }

    public class ProductCreate
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductCategory { get; set; }
        public string Manufacturer {get;set;}
        public string Description { get;set;}
        public bool IsActive { get; set; }

    }

    public class ProductDetail
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductCategory { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

    }

    public class ProductEdit
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductCategory { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}