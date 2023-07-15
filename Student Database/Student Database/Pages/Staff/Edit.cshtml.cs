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
                                staffInfo.date_of_birth = reader.GetDateTime(7);
                                staffInfo.joining_date = reader.GetDateTime(8);
                                staffInfo.salary = reader.GetDecimal(9);
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
            staffInfo.date_of_birth = DateTime.Parse(Request.Form["date_of_birth"]);
            staffInfo.joining_date = DateTime.Parse(Request.Form["joining_date"]);
            staffInfo.salary = decimal.Parse(Request.Form["salary"]);

            if (staffInfo.name.Length == 0 || staffInfo.email.Length == 0 || staffInfo.department_id.Length == 0 || staffInfo.nationality.Length == 0)
            {
                errorMessage = "Enter data into all fields.";
                return;
            }

            if (staffInfo.salary < 0)
            {
                errorMessage = "Salary cannot be less than zero.";
                return;
            }

            // Calculate age
            TimeSpan age_at_joining = staffInfo.joining_date - staffInfo.date_of_birth;
            int years = (int)(age_at_joining.TotalDays / 365.25);

            // Validate age
            if (years < 18)
            {
                errorMessage = "Staff member must be at least 18 years old.";
                return;
            }

            // Validate joining date
            if (staffInfo.joining_date < staffInfo.date_of_birth || staffInfo.joining_date > DateTime.Today)
            {
                errorMessage = "Joining date is erroneous, please double check the dates.";
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
                     "SET staff_name = @staff_name, department_id = @department_id, nationality = @nationality, email = @email, date_of_birth=@date_of_birth, joining_date=@joining_date, salary=@salary";

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
                        command.Parameters.AddWithValue("@date_of_birth", staffInfo.date_of_birth);
                        command.Parameters.AddWithValue("@joining_date", staffInfo.joining_date);
                        command.Parameters.AddWithValue("@salary", staffInfo.salary);
                        if (imageData != null)
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
            staffInfo.date_of_birth = DateTime.Today;
            staffInfo.joining_date = DateTime.Today;
            successMessage = "Data Successfully Entered into the DataBase";

            Response.Redirect("/Staff/Index");
        }
    }
}
