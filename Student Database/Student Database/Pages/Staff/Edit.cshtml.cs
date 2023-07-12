using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using Student_Database.Pages.Departments;
using Student_Database.Pages.Students;
using System.Data.SqlClient;

namespace Student_Database.Pages.Staff
{
    public class EditModel : PageModel
    {
        public StaffInfo staffInfo = new StaffInfo();
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            try
            {
                String staff_id = Request.Query["staff_id"];
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Code to fetch and display existing staff details
                    string fetchSql = "SELECT * FROM Staff WHERE staff_id = @staff_id";
                    using (SqlCommand fetchCommand = new SqlCommand(fetchSql, connection))
                    {
                        fetchCommand.Parameters.AddWithValue("@staff_id", staff_id);
                        using (SqlDataReader reader = fetchCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                staffInfo.id = "" + reader.GetInt32(0);
                                staffInfo.name = reader.GetString(1);
                                staffInfo.department_id = "" + reader.GetInt32(2);
                                staffInfo.nationality = reader.GetString(3);
                                staffInfo.email = reader.GetString(4);
                                byte[] imageData = (byte[])reader["image"];
                                staffInfo.ImageBase64 = Convert.ToBase64String(imageData);
                            }
                        }
                    }
                    string sql = "SELECT DISTINCT department_id, department_name FROM Department";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DepartmentInfo departmentInfo = new DepartmentInfo();
                                departmentInfo.id = ""+reader.GetInt32(0);
                                departmentInfo.name = reader.GetString(1);
                                listDepartments.Add(departmentInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception: " + x.ToString());
            }
        }

        public void OnPost(IFormFile image)
        {
            staffInfo.id = Request.Form["staff_id"];
            staffInfo.name = Request.Form["name"];
            staffInfo.email = Request.Form["email"];
            staffInfo.department_id = Request.Form["department"];
            staffInfo.nationality = Request.Form["nationality"];

            if (staffInfo.name.Length == 0 || staffInfo.email.Length == 0 || staffInfo.department_id.Length == 0 || staffInfo.nationality.Length == 0)
            {
                errorMessage = "Enter data into all fields.";
                return;
            }

            try
            {
                // Convert the image file to a byte array
                byte[] imageData;
                if (image != null)
                {
                    // Convert the image file to a byte array
                    using (var memoryStream = new MemoryStream())
                    {
                        image.CopyTo(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }
                else
                {
                    // No image selected, check if there is an existing image
                    if (!string.IsNullOrEmpty(staffInfo.ImageBase64))
                    {
                        // Use the existing image from the database
                        imageData = Convert.FromBase64String(staffInfo.ImageBase64);
                    }
                    else
                    {
                        // No image selected and no existing image, set imageData to null or an appropriate default value
                        imageData = null; // You can modify this line to use an appropriate default image if needed
                    }
                }

                // SQL code to insert staff info and image into the database
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Staff " +
                     "SET staff_name = @staff_name, department_id = @department_id, nationality = @nationality, email = @email";

                    if (imageData != null)
                    {
                        sql += ", image = @image";
                    }

                    sql += " WHERE staff_id = @staff_id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@staff_id", staffInfo.id);
                        command.Parameters.AddWithValue("@staff_name", staffInfo.name);
                        command.Parameters.AddWithValue("@email", staffInfo.email);
                        command.Parameters.AddWithValue("@department_id", staffInfo.department_id);
                        command.Parameters.AddWithValue("@nationality", staffInfo.nationality);
                        if(imageData != null)
                        {
                            command.Parameters.AddWithValue("@image", imageData);
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }


            staffInfo.name = "";
            staffInfo.email = "";
            staffInfo.department_id = "";
            staffInfo.nationality = "";
            successMessage = "Data Successfully Entered into the DataBase";

            Response.Redirect("/Staff/Index");
        }
    }
}
