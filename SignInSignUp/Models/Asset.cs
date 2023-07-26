using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int ProductId { get; set; }

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
        public Product Product { get; set; }
        public Vendor Vendor { get; set; }

        [Display(Name = "Invoice")]
        public HttpPostedFileBase InvoiceFile { get; set; }

        // New properties for storing the file path and content type
        public string InvoiceFilePath { get; set; }
        public string InvoiceContentType { get; set; }


        // Add a new property to store the path of the existing invoice file
        public string ExistingInvoiceFilePath { get; set; }

        // Add a new property to store the content type of the existing invoice file
        public string ExistingInvoiceContentType { get; set; }

        // Add a new property to store the uploaded new invoice file
        [NotMapped] // This attribute ensures that the property is not mapped to the database
        public HttpPostedFileBase NewInvoiceFile { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
    public class Vendor
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
    }

    public class AssetDetails
    {
        public int AssetID { get; set; }
        [Display(Name = "Asset Name")]
        public string AssetName { get; set; }

        [Display(Name = "Product Category")]
        public string ProductCategory { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        [Display(Name = "Vendor")]
        public string VendorName { get; set; }

        public int VendorID { get; set; }
        public int ProductID { get; set; }

        public string InvoiceFilePath { get; set; }
        public string InvoiceContentType { get; set; }

    }



}