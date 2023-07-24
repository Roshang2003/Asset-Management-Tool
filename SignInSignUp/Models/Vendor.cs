using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SignInSignUp.Models
{
    public class VendorTable
    {
        [Display(Name = "Vendor Name")]
        public string vendorName { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Phone")]
        public string phone { get; set; }
        [Display(Name = "Vendor ID")]
        public string vendorID { get; set; }
        public bool IsActive { get; set; }
    }


    public class VendorCreate
    {
        [Display(Name = "Vendor Name")]
        public string vendorName { get; set; }

        [Display(Name = "Contact Person")]
        public string name { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Phone")]
        public string phone { get; set; }

        [Display(Name = "GSTIN")]
        public string gstin { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Country")]
        public string country { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "PinCode")]
        public int pincode { get; set; }

        public bool IsActive { get; set; }
        public VendorCreate()
        {
            country = "India";
        }
    }
    
    public class VendorDetails
    {

        [Display(Name = "Vendor Name")]
        public string vendorName { get; set; }

        [Display(Name = "Contact Person")]
        public string name { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Phone")]
        public string phone { get; set; }

        [Display(Name = "GSTIN")]
        public string gstin { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Country")]
        public string country { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "PinCode")]
        public int pincode { get; set; }
        public int vendorId { get; set; }
        public bool IsActive { get; set; }
        public VendorDetails()
        {
            country = "India";
        }
    }

    public class VendorEdit
    {
        [Display(Name = "Vendor Name")]
        public string vendorName { get; set; }

        [Display(Name = "Contact Person")]
        public string name { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Phone")]
        public string phone { get; set; }

        [Display(Name = "GSTIN")]
        public string gstin { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Country")]
        public string country { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "PinCode")]
        public int pincode { get; set; }
        public bool IsActive { get; set; }
        public int vendorId { get; set; }
    }
}