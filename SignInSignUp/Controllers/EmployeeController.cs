using MySql.Data.MySqlClient;
using SignInSignUp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignInSignUp.Controllers
{
    public class EmployeeController : Controller
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        

        [HttpGet]
        public ActionResult AddEmployee()
        {
            Employee employee = new Employee();
            PopulateDropdownLists();
            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployee(Employee employee)
        {
            int userID = (int)Session["UserID"];

            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Prepare the SQL query to insert the new employee into the database
                    string query = "INSERT INTO employee (EmployeeCode, Name, MobileNo, Address, Department, Designation, Gender, JoiningDate, isActive, USER_ID) " +
                                   "VALUES (@Ecode, @Name, @MobileNo, @Address, @Department, @Designation, @Gender, @JoiningDate, @isActive, @USER_ID)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Ecode", employee.Ecode);
                        command.Parameters.AddWithValue("@Name", employee.Name);
                        command.Parameters.AddWithValue("@MobileNo", employee.MobileNo);
                        command.Parameters.AddWithValue("@Address", employee.Address);
                        command.Parameters.AddWithValue("@Department", employee.Department);
                        command.Parameters.AddWithValue("@Designation", employee.Designation);
                        command.Parameters.AddWithValue("@Gender", employee.Gender);
                        command.Parameters.AddWithValue("@JoiningDate", employee.JoiningDate);
                        command.Parameters.AddWithValue("@isActive", employee.IsActive);
                        command.Parameters.AddWithValue("@USER_ID", userID);

                        // Execute the query to insert the new employee
                        command.ExecuteNonQuery();
                    }
                }
                PopulateDropdownLists();
                return RedirectToAction("Index", "Employee"); // Redirect to home page for now
            }

            PopulateDropdownLists();
            return View(employee);
        }

        // Helper method to populate the Department and Gender dropdown lists
        private void PopulateDropdownLists()
        {
            List<SelectListItem> departmentList = new List<SelectListItem>
            {
                new SelectListItem { Value = "IT", Text = "IT" },
                new SelectListItem { Value = "Accounts", Text = "Accounts" },
                new SelectListItem { Value = "Sales", Text = "Sales" },
                new SelectListItem { Value = "Operations", Text = "Operations" },
                new SelectListItem { Value = "Logistics", Text = "Logistics" },
                new SelectListItem { Value = "Others", Text = "Others" }
            };

            List<SelectListItem> genderList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Male", Text = "Male" },
                new SelectListItem { Value = "Female", Text = "Female" },
                new SelectListItem { Value = "Others", Text = "Others" }
            };

            // Pass the dropdown lists to the view using ViewBag
            ViewBag.DepartmentList = departmentList;
            ViewBag.GenderList = genderList;
        }
    }
}