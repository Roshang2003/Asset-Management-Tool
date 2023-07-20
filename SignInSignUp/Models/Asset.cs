﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Web;

namespace SignInSignUp.Models
{
    public class Asset
    {
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Please enter the Asset Name")]
        [Display(Name = "Asset Name")]
        public string AssetName { get; set; }

        [Display(Name = "Product Category")]
        public string ProductCategory { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "Please enter the Serial Number")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Please enter the Description")]
        public string Description { get; set; }

        public int Price { get; set; }

        [Display(Name ="Vendor")]
        public int VendorId { get; set; }
        public int UserID { get; set; }
        public List<string> ProductCategories { get; set; }
        public List<Product> Products { get; set; }
        public List<Vendor> Vendors { get; set; }

    }

    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
    public class Vendor
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
    }
}