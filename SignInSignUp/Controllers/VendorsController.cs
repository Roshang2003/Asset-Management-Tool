using MySql.Data.MySqlClient;
using SignInSignUp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.Mvc;

namespace SignInSignUp.Controllers
{
    public class VendorsController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        // GET: Vendors
        [HttpGet]
        public ActionResult Index()
        {
            int loggedInUserID = (int)Session["UserID"];
            List<VendorTable> vendors = GetVendorsByUserID(loggedInUserID);

            return View(vendors);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            VendorDetails vendor = GetVendorDetail(id);

            return View(vendor);
        }


        [HttpGet]
        public ActionResult Create()
        {
            VendorCreate vendor=new VendorCreate();
            //vendor.country = "India";
            return View(vendor);
        }

        [HttpPost]
        public ActionResult Create(VendorCreate vendor)
        {
            if (ModelState.IsValid)
            {
                // Save the new user to the database
                if (AddVendorToTable(vendor))
                {
                    // Redirect to the login page or desired destination
                    return RedirectToAction("Index","Vendors");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add vendor");
                }

            }
            return View(vendor);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            VendorDetails vendorDetail = GetVendorDetail(id);

            VendorEdit vendorEdit = new VendorEdit()
            {
                vendorId = vendorDetail.vendorId,
                vendorName = vendorDetail.vendorName,
                name = vendorDetail.name,
                email = vendorDetail.email,
                phone = vendorDetail.phone,
                designation = vendorDetail.designation,
                gstin = vendorDetail.gstin,
                country = vendorDetail.country,
                state = vendorDetail.state,
                city = vendorDetail.city,
                address = vendorDetail.address,
                pincode = vendorDetail.pincode,
                IsActive = vendorDetail.IsActive,
            };
            return View(vendorEdit);
        }

        [HttpPost]
        public ActionResult Edit(VendorEdit vendorEdit)
        {
            if (ModelState.IsValid)
            {
                
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE vendor SET Vendor_Name=@Vendor_Name, Email=@Email, GSTIN_No=@GSTIN_No, Contact_No=@Contact_No, Contact_Person=@Contact_Person, Designation=@Designation, Country=@Country, State=@State, City=@City, Pin_Code=@Pin_Code, Address=@Address WHERE Vendor_ID=@Vendor_ID";

                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                    {

                        updateCommand.Parameters.AddWithValue("@Vendor_ID", vendorEdit.vendorId);
                        updateCommand.Parameters.AddWithValue("@Vendor_Name", vendorEdit.vendorName);
                        updateCommand.Parameters.AddWithValue("@Email", vendorEdit.email);
                        updateCommand.Parameters.AddWithValue("@GSTIN_No", vendorEdit.gstin);
                        updateCommand.Parameters.AddWithValue("@Contact_No", vendorEdit.phone);
                        updateCommand.Parameters.AddWithValue("@Contact_Person", vendorEdit.name);
                        updateCommand.Parameters.AddWithValue("@Designation", vendorEdit.designation);
                        updateCommand.Parameters.AddWithValue("@Country", vendorEdit.country);
                        updateCommand.Parameters.AddWithValue("@State", vendorEdit.state);  
                        updateCommand.Parameters.AddWithValue("@City", vendorEdit.city);
                        updateCommand.Parameters.AddWithValue("@Pin_Code", vendorEdit.pincode);
                        updateCommand.Parameters.AddWithValue("@Address", vendorEdit.address);

                        updateCommand.ExecuteNonQuery();

                        return RedirectToAction("Index", "Vendors");
                    }
                }
            }
            return View(vendorEdit);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            // Show the confirmation dialog before deleting the vendor
            VendorDetails vendorDetail = GetVendorDetail(id);
            return View(vendorDetail);
        }
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            // Perform the actual deletion of the vendor
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "UPDATE vendor set IsActive=@IsActive WHERE Vendor_ID = @Vendor_ID";
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);
                deleteCommand.Parameters.AddWithValue("@Vendor_ID", id);
                deleteCommand.Parameters.AddWithValue("@IsActive", false);
                deleteCommand.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "Vendors");
        }

        public List<VendorTable> GetVendorsByUserID(int userID)
        {
            List<VendorTable> vendors = new List<VendorTable>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Vendor WHERE User_ID = @User_ID and IsActive=@IsActive";
                    command.Parameters.AddWithValue("@User_ID", userID);
                    command.Parameters.AddWithValue("@IsActive", true);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VendorTable vendor = new VendorTable
                            {
                                name = reader["Contact_Person"].ToString(),
                                email = reader["Email"].ToString(),
                                vendorName = reader["Vendor_Name"].ToString(),
                                phone = reader["Contact_No"].ToString(),
                                vendorID = reader["Vendor_ID"].ToString()
                            };
                            vendors.Add(vendor);
                        }
                    }
                }
            }
            return vendors;
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

        private bool AddVendorToTable(VendorCreate vendor)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                int userID = (int)Session["UserID"];

                connection.Open();

                string query = "INSERT INTO vendor (Vendor_Name,IsActive,Contact_Person,Email,GSTIN_No,Contact_No,User_ID,Designation,Country,State,City,Pin_Code,Address) VALUES (@Vendor_Name,@IsActive,@Contact_Person,@Email,@GSTIN_No,@Contact_No,@User_ID,@Designation,@Country,@State,@City,@Pin_Code,@Address)" ;
                
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Vendor_Name", vendor.vendorName);
                    command.Parameters.AddWithValue("@Contact_Person", vendor.name);
                    command.Parameters.AddWithValue("@Email", vendor.email);
                    command.Parameters.AddWithValue("@GSTIN_No", vendor.gstin);
                    command.Parameters.AddWithValue("@Contact_No", vendor.phone);
                    command.Parameters.AddWithValue("@User_ID", userID);
                    command.Parameters.AddWithValue("@Designation", vendor.designation);
                    command.Parameters.AddWithValue("@Country", vendor.country);
                    command.Parameters.AddWithValue("@State", vendor.state);
                    command.Parameters.AddWithValue("@City", vendor.city);
                    command.Parameters.AddWithValue("@Pin_Code", vendor.pincode);
                    command.Parameters.AddWithValue("@Address", vendor.address);
                    command.Parameters.AddWithValue("@IsActive", true);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public VendorDetails GetVendorDetail(int id)
        {
            VendorDetails vendor = new VendorDetails();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Vendor_Name, Contact_Person, IFNULL(Email,'N/A') Email, IsActive, Contact_No, IFNULL(Designation,'N/A') Designation , Country, City, State, IFNULL(Address,'N/A') Address, IFNULL(Pin_Code,0) Pin_Code,IFNULL(GSTIN_No,'N/A') GSTIN_No FROM vendor WHERE Vendor_ID = @Vendor_ID";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Vendor_ID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vendor.vendorName = reader.GetString("Vendor_Name");
                            vendor.name = reader.GetString("Contact_Person");
                            vendor.email = reader.GetString("Email");
                            vendor.phone = reader.GetString("Contact_No");
                            vendor.designation = reader.GetString("Designation");
                            vendor.country = reader.GetString("Country");
                            vendor.city = reader.GetString("City");
                            vendor.state = reader.GetString("State");
                            vendor.address = reader.GetString("Address");
                            vendor.pincode = reader.GetInt32("Pin_Code");
                            vendor.vendorId = id;
                            vendor.gstin = reader.GetString("GSTIN_No");
                            vendor.IsActive = reader.GetBoolean("IsActive");
                        }
                    }
                }
            }
            return vendor;
        }
    }
}