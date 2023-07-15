using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using Student_Database.Pages.Departments;
using Student_Database.Pages.Students;
using System;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace Student_Database.Pages.Staff
{
    public class CreateModel : PageModel
    {
        public StaffInfo staffInfo = new StaffInfo();
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            try
            {
                // Retrieve department data from the database
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
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
            staffInfo.name = Request.Form["name"];
            staffInfo.email = Request.Form["email"];
            staffInfo.department_id = Request.Form["department"];
            staffInfo.nationality = Request.Form["nationality"];
            // Retrieve the new field values
            staffInfo.date_of_birth = DateTime.Parse(Request.Form["date_of_birth"]);
            staffInfo.joining_date = DateTime.Parse(Request.Form["joining_date"]);
            staffInfo.salary = decimal.Parse(Request.Form["salary"]);

            if (staffInfo.name.Length == 0 || staffInfo.email.Length == 0 || staffInfo.department_id.Length == 0 || staffInfo.nationality.Length == 0 || image == null)
            {
                errorMessage = "Enter data into all fields and select an image.";
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

            // Convert the image file to a byte array
            byte[] imageData;
            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                imageData = memoryStream.ToArray();
            }


            try
            {
                // SQL code to insert staff info and image into the database
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string maxIdSql = "SELECT MAX(staff_id) FROM Staff";
                    int? maxId = null; // Use nullable int to handle no rows scenario
                    using (SqlCommand maxIdCommand = new SqlCommand(maxIdSql, connection))
                    {
                        var result = maxIdCommand.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            maxId = Convert.ToInt32(result);
                        }
                    }
                    int newStaffId = maxId.HasValue ? maxId.Value + 1 : 1; // Set initial staff_id to 1 if maxId is null
                    string sql = "INSERT INTO Staff " +
                                 "(staff_id, staff_name, department_id, nationality, email, image, date_of_birth, joining_date, salary) " +
                                 "VALUES (@staff_id, @staff_name, @department_id, @nationality, @email, @image, @date_of_birth, @joining_date, @salary)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@staff_id", newStaffId);
                        command.Parameters.AddWithValue("@staff_name", staffInfo.name);
                        command.Parameters.AddWithValue("@email", staffInfo.email);
                        command.Parameters.AddWithValue("@department_id", staffInfo.department_id);
                        command.Parameters.AddWithValue("@nationality", staffInfo.nationality);
                        command.Parameters.AddWithValue("@image", imageData);
                        command.Parameters.AddWithValue("@date_of_birth", staffInfo.date_of_birth);
                        command.Parameters.AddWithValue("@joining_date", staffInfo.joining_date);
                        command.Parameters.AddWithValue("@salary", staffInfo.salary);
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