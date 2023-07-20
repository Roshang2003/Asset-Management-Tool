using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignInSignUp.Models
{
    public class VendorTable
    {
        public string vendorName { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string vendorID { get; set; }
        public bool IsActive { get; set; }
    }


    public class VendorCreate
    {
        public string vendorName { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string gstin { get; set; }
        public string designation { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string address { get; set; }
        public int pincode { get; set; }
        public bool IsActive { get; set; }
        public VendorCreate()
        {
            country = "India";
        }
    }
    
    public class VendorDetails
    {
        public string vendorName { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string gstin { get; set; }
        public string designation { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string address { get; set; }
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
        public string vendorName { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string gstin { get; set; }
        public int vendorId { get; set; }
        public string designation { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string address { get; set; }
        public int pincode { get; set; }
        public bool IsActive { get; set; }
    }
}