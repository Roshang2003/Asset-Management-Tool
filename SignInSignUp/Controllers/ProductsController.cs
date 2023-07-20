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
    public class ProductsController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        // GET: Product
        [HttpGet]
        public ActionResult Index()
        {
            int loggedInUserID = (int)Session["UserID"];
            List<ProductsTable> products = GetProductsByUserID(loggedInUserID);
            return View(products);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductCreate product)
        {
            if (ModelState.IsValid)
            {
                // Save the new user to the database
                if (AddProductToTable(product))
                {
                    // Redirect to the login page or desired destination
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add product");
                }

            }
            return View(product);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            ProductDetail productDetail = GetProductDetail(id);
            ProductEdit productEdit = new ProductEdit()
            {
                ProductID = productDetail.ProductID,
                ProductName = productDetail.ProductName,
                ProductCategory = productDetail.ProductCategory,
                ProductType = productDetail.ProductType,
                Description = productDetail.Description,
                Manufacturer = productDetail.Manufacturer,
                IsActive = productDetail.IsActive,
            };
            return View(productEdit);
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            ProductDetail product = GetProductDetail(id);
            return View(product);
        }


        [HttpPost]
        public ActionResult Edit(ProductEdit productEdit)
        {
            if (ModelState.IsValid)
            {

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE product SET Name=@Name, Category=@Category, Type=@Type, Manufacturer=@Manufacturer, Description=@Description WHERE ProductID=@ProductID";

                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@ProductID", productEdit.ProductID);
                        updateCommand.Parameters.AddWithValue("@Name", productEdit.ProductName);
                        updateCommand.Parameters.AddWithValue("@Category", productEdit.ProductCategory);
                        updateCommand.Parameters.AddWithValue("@Type", productEdit.ProductType);
                        updateCommand.Parameters.AddWithValue("@Manufacturer", productEdit.Manufacturer);
                        updateCommand.Parameters.AddWithValue("@Description", productEdit.Description);

                        updateCommand.ExecuteNonQuery();

                        return RedirectToAction("Index", "Products");
                    }
                }
            }
            return View(productEdit);
        }
        public List<ProductsTable> GetProductsByUserID(int userID)
        {
            List<ProductsTable> products = new List<ProductsTable>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM product WHERE User_ID = @User_ID and IsActive=@IsActive";
                    command.Parameters.AddWithValue("@User_ID", userID);
                    command.Parameters.AddWithValue("@IsActive", true);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductsTable vendor = new ProductsTable
                            {
                                ProductName = reader["Name"].ToString(),
                                ProductType = reader["Type"].ToString(),
                                ProductCategory = reader["Category"].ToString(),
                                ProductID = reader.GetInt32("ProductID"),
                        };
                            products.Add(vendor);
                        }
                    }
                }
            }
            return products;
        }


        private bool AddProductToTable(ProductCreate product)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                int userID = (int)Session["UserID"];

                connection.Open();

                string query = "INSERT INTO product (Type,Category,Name,Manufacturer,Description,User_ID,IsActive) VALUES (@Type,@Category,@Name,@Manufacturer,@Description,@User_ID,@IsActive)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", product.ProductType);
                    command.Parameters.AddWithValue("@Category", product.ProductCategory);
                    command.Parameters.AddWithValue("@Name", product.ProductName);
                    command.Parameters.AddWithValue("@Manufacturer", product.Manufacturer);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@IsActive", true);
                    command.Parameters.AddWithValue("@User_ID", userID);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }


        ProductDetail GetProductDetail(int id)
        {
            ProductDetail product = new ProductDetail();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ProductID,Type,Category,Name,Manufacturer,IFNULL(Description,'N/A') Description,IsActive FROM product WHERE ProductID = @ProductID";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product.ProductID = id;
                            product.ProductType = reader.GetString("Type");
                            product.ProductCategory = reader.GetString("Category");
                            product.ProductName = reader.GetString("Name");
                            product.Manufacturer = reader.GetString("Manufacturer");
                            product.Description = reader.GetString("Description");
                            product.IsActive = reader.GetBoolean("IsActive");
                        }
                    }
                }
            }
            return product;
        }

        public ActionResult DeleteConfirmed(int id)
        {
            // Perform the actual deletion of the vendor
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "UPDATE product set IsActive=@IsActive WHERE ProductID = @ProductID";
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);
                deleteCommand.Parameters.AddWithValue("@ProductID", id);
                deleteCommand.Parameters.AddWithValue("@IsActive", false);
                deleteCommand.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "Products");
        }

    }
}