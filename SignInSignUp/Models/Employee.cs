using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignInSignUp.Models
{
    public class Employee
    {
        [Key]
        public int EmpID { get; set; }

        [Required]
        [Display(Name = "Employee Code")]
        public int Ecode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Designation { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }

        public bool IsActive { get; set; } = true; // Default value is true
    }
}
