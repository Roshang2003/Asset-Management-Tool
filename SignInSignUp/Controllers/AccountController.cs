using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using SignInSignUp.Models;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Security;

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
                string name = GetUserNameByEmail(model.Email); // Retrieve the name from the database using the email
                return RedirectToAction("Index", "Account", new { name=name });
                //return RedirectToAction("Index", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or Password");
            }
        }

        return View(model);
    }

    [HttpGet]
    public ActionResult Index(string name)
    {
        return View((object)name);
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
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Failed to create user");
            }
        }

        return View(model);
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
                return result > 0;
            }
        }
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

}
