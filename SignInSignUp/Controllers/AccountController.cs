using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using SignInSignUp.Models;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Security;
using System;
using System.Xml.Linq;

public class AccountController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    public ActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Validate user credentials
            if (ValidateUser(model.Email, model.Password))
            {

                // Authentication successful
                
                // Redirect to the home page or desired destination

                FormsAuthentication.SetAuthCookie(model.Email, false);

                return RedirectToAction("Index", "Account");
                
                //return RedirectToAction("Index", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or Password");
            }
        }

        return View(model);
    }


    //public ActionResult Index()
    //{
    //    return View();
    //}

    public ActionResult Logout()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Login");
    }

    // GET: /Account/Signup
    [AllowAnonymous]
    public ActionResult Signup()
    {
        return View();
    }

    // POST: /Account/Signup
    [HttpPost]
    [AllowAnonymous]
    public ActionResult Signup(SignupViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create a new user
            var newUser = new User
            {
                Name = model.Name,
                Age = model.Age,
                Email = model.Email,
                Password = model.Password
            };

            // Save the new user to the database
            if (CreateUser(newUser))
            {
                // Redirect to the login page or desired destination
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Failed to create user");
            }
        }

        return View(model);
    }

    [HttpGet]
    public ActionResult Index()
    {
        //int vendorCount = (int)ViewBag.VendorCount;
        string name = GetUserNameByEmail(User.Identity.Name); // Retrieve the name from the database using the email

        Session["UserName"] = name;

        int loggedInUserID = GetUserIDByEmail(User.Identity.Name);
        Session["UserID"] = loggedInUserID;

        DashboardDetails dashboardDetails = new DashboardDetails()
        {
            Vendors = CountVendors(loggedInUserID),
            Products = CountProducts(loggedInUserID),
        };

        return View(dashboardDetails);
    }

    private string GetUserNameByEmail(string email)
    {
        //string connectionString = connectionString; // Replace with your connection string

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Name FROM trial1login WHERE Email = @Email";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                if (result != null)
                {
                    return result.ToString();
                }
            }
        }
        return string.Empty;
    }


    // Helper method to validate user credentials
    private bool ValidateUser(string email, string password)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            var commandText = "SELECT COUNT(*) FROM trial1login WHERE Email = @Email AND Password = @Password";
            using (var command = new MySqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                var result = (long)command.ExecuteScalar();

                if(result > 0)
                {
                    return true;
                }
                
            }
        }
        return false;
    }

    // Helper method to create a new user
    private bool CreateUser(User user)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            var commandText = "INSERT INTO trial1login (Name, Age, Email, Password) VALUES (@Name, @Age, @Email, @Password)";
            
            using (var command = new MySqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Age", user.Age);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }


    public int GetUserIDByEmail(string email)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT ID FROM trial1login WHERE Email = @Email";
                command.Parameters.AddWithValue("@Email", email);

                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
        }

        // User with the specified email not found
        return -1; // Or any other default value or error handling mechanism
    }

    int CountVendors(int loggedInUserID)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Count(*) FROM vendor WHERE User_ID = @User_ID and IsActive=@IsActive";
                command.CommandText = "SELECT Count(*) FROM vendor WHERE User_ID = @User_ID and IsActive=@IsActive";
                command.Parameters.AddWithValue("@User_ID", loggedInUserID);
                command.Parameters.AddWithValue("@IsActive", true);
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
        }
        return 0;
    }

    int CountProducts(int loggedInUserID)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Count(*) FROM product WHERE USER_ID = @USER_ID and IsActive=@IsActive";
                command.Parameters.AddWithValue("@USER_ID", loggedInUserID);
                command.Parameters.AddWithValue("@IsActive", true);

                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
        }
        return 0;
    }

}
