using MySql.Data.MySqlClient;
using SignInSignUp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
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

        [HttpGet]
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

        // POST: Asset/Add
        [HttpPost]
        public ActionResult Add(Asset asset)
        {
            if (ModelState.IsValid)
            {
                // Upload the invoice file if it exists
                if (asset.InvoiceFile != null && asset.InvoiceFile.ContentLength > 0)
                {
                    // Specify the directory where the file will be saved
                    var uploadDirectory = Server.MapPath("~/App_Data/Invoices");

                    // Generate a unique file name to avoid conflicts
                    var fileName = Path.GetFileNameWithoutExtension(asset.InvoiceFile.FileName);
                    var fileExtension = Path.GetExtension(asset.InvoiceFile.FileName);
                    var uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

                    // Combine the directory and file name to get the full path
                    var filePath = Path.Combine(uploadDirectory, uniqueFileName);

                    // Save the file to the specified location
                    asset.InvoiceFile.SaveAs(filePath);

                    // Save the file path and content type in the model
                    asset.InvoiceFilePath = uniqueFileName;
                    asset.InvoiceContentType = asset.InvoiceFile.ContentType;
                }

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

        [HttpGet]
        public ActionResult Edit(int assetID)
        {
            // Retrieve the asset details from the database using the assetID
            Asset asset = GetAssetById(assetID);

            // Populate the dropdown lists
            asset.ProductCategories = GetProductCategories();
            asset.Products = GetProductsByCategoryFromDatabase(asset.ProductCategory);
            asset.Vendors = GetVendors();

            // Pass the asset to the view for editing
            return View(asset);
        }

        // POST: Asset/Edit
        [HttpPost]
        public ActionResult Edit(Asset asset, HttpPostedFileBase NewInvoiceFile)
        {
            if (ModelState.IsValid)
            {
                if (NewInvoiceFile != null && NewInvoiceFile.ContentLength > 0)
                {

                    // Specify the directory where the file will be saved
                    var uploadDirectory = Server.MapPath("~/App_Data/Invoices");
                    // User uploaded a new invoice file
                    string fileName = Path.GetFileName(NewInvoiceFile.FileName);
                    string fileExtension = Path.GetExtension(fileName);
                    // Generate a unique file name to avoid conflicts
                    var uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    var filePath = Path.Combine(uploadDirectory, uniqueFileName);
                    NewInvoiceFile.SaveAs(filePath);

                    asset.InvoiceFilePath = uniqueFileName; // Store only the unique file name
                    asset.InvoiceContentType = NewInvoiceFile.ContentType;

                }

                // Update the asset in the database
                UpdateAssetInDatabase(asset);

                return RedirectToAction("Index");
            }

            // If the model is not valid, return to the Edit view with the provided asset
            return View(asset);
        }



        private Asset GetAssetById(int assetId)
        {
            int? userID = Session["UserID"] as int?;
            if (userID == null)
            {
                // Handle the case where UserID is null or invalid
                RedirectToAction("Login", "Account");
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT AssetName, ProductCategory, ProductID, SerialNumber, Description, Price, VendorID, InvoiceFilePath, InvoiceContentType FROM asset WHERE AssetID = @AssetID AND USER_ID = @UserID";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AssetID", assetId);
                    command.Parameters.AddWithValue("@UserID", userID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Asset asset = new Asset
                            {
                                AssetId = assetId,
                                AssetName = reader.GetString("AssetName"),
                                ProductCategory = reader.GetString("ProductCategory"),
                                ProductId = reader.GetInt32("ProductID"),
                                SerialNumber = reader.GetString("SerialNumber"),
                                Description = reader.GetString("Description"),
                                Price = reader.GetInt32("Price"),
                                VendorId = reader.GetInt32("VendorID"),
                                InvoiceFilePath = reader.IsDBNull(reader.GetOrdinal("InvoiceFilePath")) ? null : reader.GetString("InvoiceFilePath"),
                                InvoiceContentType = reader.IsDBNull(reader.GetOrdinal("InvoiceContentType")) ? null : reader.GetString("InvoiceContentType")
                            };

                            // Fetch the associated Product based on ProductId
                            asset.Product = GetProductById(asset.ProductId);
                            asset.Vendor = GetVendorById(asset.VendorId);

                            return asset;
                        }
                    }
                }
            }

            return null; // Return null if asset with the given ID is not found or doesn't belong to the current user
        }

        // Helper method to update an asset in the database
        private void UpdateAssetInDatabase(Asset asset)
        {
            int? userID = Session["UserID"] as int?;
            if (userID == null)
            {
                // Handle the case where UserID is null or 
                RedirectToAction("Login", "Account");
            }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("UPDATE asset SET AssetName = @AssetName, ProductCategory = @ProductCategory, ProductID = @ProductID, SerialNumber = @SerialNumber, Description = @Description, Price = @Price, VendorID = @VendorID, InvoiceFilePath = @InvoiceFilePath, InvoiceContentType = @InvoiceContentType WHERE AssetID = @AssetID AND USER_ID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@AssetName", asset.AssetName);
                    command.Parameters.AddWithValue("@ProductCategory", asset.ProductCategory);
                    command.Parameters.AddWithValue("@ProductID", asset.ProductId);
                    command.Parameters.AddWithValue("@SerialNumber", asset.SerialNumber);
                    command.Parameters.AddWithValue("@Description", asset.Description);
                    command.Parameters.AddWithValue("@Price", asset.Price);
                    command.Parameters.AddWithValue("@VendorID", asset.VendorId);
                    command.Parameters.AddWithValue("@AssetID", asset.AssetId);
                    command.Parameters.AddWithValue("@UserID", userID);

                    // Parameters for Invoice file
                    command.Parameters.AddWithValue("@InvoiceFilePath", asset.InvoiceFilePath ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@InvoiceContentType", asset.InvoiceContentType ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
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
                // Handle the case where UserID is null or 
                 RedirectToAction("Login", "Account");
            }
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("INSERT INTO asset (AssetName, ProductCategory, ProductID, SerialNumber, Description, Price, USER_ID, VendorID,InvoiceFilePath,InvoiceContentType) VALUES (@AssetName, @ProductCategory, @ProductID, @SerialNumber, @Description, @Price, @UserID,@VendorID,@InvoiceFilePath,@InvoiceContentType)", connection))
                {

                    command.Parameters.AddWithValue("@AssetName", asset.AssetName);
                    command.Parameters.AddWithValue("@ProductCategory", asset.ProductCategory);
                    command.Parameters.AddWithValue("@ProductID", asset.ProductId);
                    command.Parameters.AddWithValue("@SerialNumber", asset.SerialNumber);
                    command.Parameters.AddWithValue("@Description", asset.Description);
                    command.Parameters.AddWithValue("@Price", asset.Price);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@VendorID", asset.VendorId);

                    // Parameters for Invoice file
                    command.Parameters.AddWithValue("@InvoiceFilePath", asset.InvoiceFilePath ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@InvoiceContentType", asset.InvoiceContentType ?? (object)DBNull.Value);

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

                string query = "SELECT AssetName,AssetID,SerialNumber,ProductCategory,Price,Description, VendorID, ProductID, IFNULL(InvoiceFilePath,'') InvoiceFilePath, IFNULL(InvoiceContentType,'') InvoiceContentType FROM asset WHERE AssetID = @AssetID";

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

                            asset.InvoiceFilePath = reader.GetString("InvoiceFilePath");
                            asset.InvoiceContentType = reader.GetString("InvoiceContentType");
                        }
                    }
                }
            }
            return asset;
        }

        public ActionResult DownloadInvoice(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return HttpNotFound();
            }

            // Define the path to the folder where invoice files are stored on the server
            string invoiceFolderPath = Server.MapPath("~/App_Data/Invoices");

            // Combine the folder path with the file name to get the full path of the invoice file
            string filePath = Path.Combine(invoiceFolderPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            // Get the file's content type (MIME type)
            string contentType = MimeMapping.GetMimeMapping(fileName);

            // Serve the file for download
            return File(filePath, contentType, fileName);
        }

    }
}