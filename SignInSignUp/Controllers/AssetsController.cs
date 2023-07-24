﻿using MySql.Data.MySqlClient;
using SignInSignUp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignInSignUp.Controllers
{
    public class AssetsController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        // GET: Assets
        public ActionResult Index()
        {
            List<Asset> assets = GetAssetsFromDatabase();
            return View(assets);
        }

        public ActionResult AssignedAssets()
        {
            return View();
        }

        public ActionResult Add()
        {
            Asset model = new Asset
            {
                ProductCategories = GetProductCategories(),
                Products = new List<Product>(),
                Vendors = GetVendors()
            };

            return View(model);
        }

        [HttpGet]
        public JsonResult GetProductsByCategory(string category)
        {
            var products = GetProductsByCategoryFromDatabase(category);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Details(int assetID)
        {
            AssetDetails asset = GetAssetDetails(assetID);

            return View(asset);
        }

        // POST: Asset/Add
        [HttpPost]
        public ActionResult Add(Asset asset)
        {
            if (ModelState.IsValid)
            {
                // Add asset to database
                AddAssetToDatabase(asset);

                // Redirect to success page or another action
                return RedirectToAction("Index", "Assets");
            }

            asset.ProductCategories = GetProductCategories();
            asset.Products = GetProductsByCategoryFromDatabase(asset.ProductCategory);
            asset.Vendors = GetVendors();
            return View(asset);
        }

        // Helper method to retrieve vendors from the database
        private List<Vendor> GetVendors()
        {
            List<Vendor> vendors = new List<Vendor>();
            int userID = (int)Session["UserID"];
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Vendor_ID, Vendor_Name FROM vendor WHERE User_ID=@User_ID and IsActive=true";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@User_ID", userID);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int vendorId = reader.GetInt32("Vendor_ID");
                    string vendorName = reader.GetString("Vendor_Name");

                    vendors.Add(new Vendor { VendorId = vendorId, VendorName = vendorName });
                }

                reader.Close();
            }

            return vendors;
        }

        private List<string> GetProductCategories()
        {
            List<string> categories = new List<string>();
            int userID = (int)Session["UserID"];
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("SELECT DISTINCT Category FROM product WHERE USER_ID=@User_ID", connection))
                {
                    command.Parameters.AddWithValue("@User_ID", userID);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader.GetString("Category");
                            categories.Add(category);
                        }
                    }
                }
            }
            return categories;
        }

        private List<Product> GetProductsByCategoryFromDatabase(string category)
        {
            List<Product> products = new List<Product>();
            int userID = (int)Session["UserID"];
            if (string.IsNullOrEmpty(category))
            {
                return products;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("SELECT ProductID, Name FROM product WHERE Category = @Category and USER_ID=@User_ID", connection))
                {
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@User_ID", userID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int productId = reader.GetInt32("ProductID");
                            string productName = reader.GetString("Name");

                            products.Add(new Product { ProductId = productId, ProductName = productName });
                        }
                    }
                }
            }
            return products;
        }

        private void AddAssetToDatabase(Asset asset)
        {
            int? userID = Session["UserID"] as int?;
            if (userID == null)
            {
                // Handle the case where UserID is null or invalid
                // For example, you can redirect the user to a login page
                 RedirectToAction("Login", "Account");
            }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("INSERT INTO asset (AssetName, ProductCategory, ProductID, SerialNumber, Description, Price, USER_ID, VendorID) VALUES (@AssetName, @ProductCategory, @ProductID, @SerialNumber, @Description, @Price, @UserID,@VendorID)", connection))
                {

                    command.Parameters.AddWithValue("@AssetName", asset.AssetName);
                    command.Parameters.AddWithValue("@ProductCategory", asset.ProductCategory);
                    command.Parameters.AddWithValue("@ProductID", asset.ProductId);
                    command.Parameters.AddWithValue("@SerialNumber", asset.SerialNumber);
                    command.Parameters.AddWithValue("@Description", asset.Description);
                    command.Parameters.AddWithValue("@Price", asset.Price);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@VendorID", asset.VendorId);

                    command.ExecuteNonQuery();
                }
            }
        }

        private List<Asset> GetAssetsFromDatabase()
        {
            int? userID = Session["UserID"] as int?;
            if (userID == null)
            {
                // Handle the case where UserID is null or invalid
                // For example, you can redirect the user to a login page
                RedirectToAction("Login", "Account");
            }

            List<Asset> assets = new List<Asset>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT AssetID, AssetName, SerialNumber, ProductID,VendorID FROM Asset WHERE USER_ID=@USER_ID";
                
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@User_ID", userID);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Asset asset = new Asset
                    {
                        AssetId = reader.GetInt32("AssetID"),
                        AssetName = reader.GetString("AssetName"),
                        SerialNumber = reader.GetString("SerialNumber"),
                        ProductId = reader.GetInt32("ProductID"),
                        VendorId = reader.GetInt32("VendorID")
                    };

                    // Fetch the associated Product based on ProductId
                    asset.Product = GetProductById(asset.ProductId);
                    asset.Vendor = GetVendorById(asset.VendorId);
                    assets.Add(asset);
                }

                reader.Close();
            }

            return assets;
        }

        // Helper method to retrieve a product by its ID
        private Product GetProductById(int productId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Name FROM product WHERE ProductID = @ProductID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productId);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Product product = new Product
                    {
                        ProductName = reader.GetString("Name")
                    };
                    reader.Close();
                    return product;
                }

                reader.Close();
                return null;
            }
        }
        
        private Vendor GetVendorById(int vendorID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Vendor_Name FROM vendor WHERE Vendor_ID = @Vendor_ID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Vendor_ID", vendorID);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Vendor vendor = new Vendor
                    {
                        VendorId=vendorID,
                        VendorName = reader.GetString("Vendor_Name")
                    };
                    reader.Close();
                    return vendor;
                }

                reader.Close();
                return null;
            }
        }

        AssetDetails GetAssetDetails(int assetID)
        {
            AssetDetails asset = new AssetDetails();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT AssetName,AssetID,SerialNumber,ProductCategory,Price,Description, VendorID, ProductID FROM asset WHERE AssetID = @AssetID";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AssetID", assetID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            asset.AssetID = reader.GetInt32("AssetID");
                            asset.AssetName = reader.GetString("AssetName");
                            asset.ProductCategory = reader.GetString("ProductCategory");
                            asset.Price = reader.GetInt32("Price");
                            asset.SerialNumber = reader.GetString("SerialNumber");
                            asset.Description = reader.GetString("Description");
                            asset.VendorID = reader.GetInt32("VendorID");
                            asset.ProductID = reader.GetInt32("ProductID");
                            asset.ProductName = GetProductById(asset.ProductID).ProductName;
                            asset.VendorName = GetVendorById(asset.VendorID).VendorName;
                        }
                    }
                }
            }
            return asset;
        }
    }
}