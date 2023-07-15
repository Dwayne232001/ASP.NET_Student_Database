using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using Student_Database.Pages.Departments;
using Student_Database.Pages.Staff;
using System.Data.SqlClient;

namespace Student_Database.Pages.Students
{
    public class EditModel : PageModel
    {
        public StudentInfo studentInfo = new StudentInfo();
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            try
            {
                String student_id = Request.Query["student_id"];
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Code to fetch and display existing student details
                    string sql = "SELECT * FROM Student WHERE student_id = @student_id";
                    using (SqlCommand fetchCommand = new SqlCommand(sql, connection))
                    {
                        fetchCommand.Parameters.AddWithValue("@student_id", student_id);
                        using (SqlDataReader reader = fetchCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentInfo.id = "" + reader.GetInt32(0);
                                studentInfo.name = reader.GetString(1);
                                studentInfo.department_id = "" + reader.GetInt32(2);
                                studentInfo.nationality = reader.GetString(3);
                                studentInfo.email = reader.GetString(4);
                                byte[] imageData = (byte[])reader["image"];
                                studentInfo.ImageBase64 = Convert.ToBase64String(imageData);
                                studentInfo.date_of_birth = reader.GetDateTime(7);
                                studentInfo.joining_date = reader.GetDateTime(8);
                            }
                        }
                    }
                    sql = "SELECT DISTINCT department_id, department_name FROM Department";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DepartmentInfo departmentInfo = new DepartmentInfo();
                                departmentInfo.id = "" + reader.GetInt32(0);
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
            studentInfo.id = Request.Form["student_id"];
            studentInfo.name = Request.Form["name"];
            studentInfo.department_id = Request.Form["department"];
            studentInfo.email = Request.Form["email"];
            studentInfo.nationality = Request.Form["nationality"];
            studentInfo.date_of_birth = DateTime.Parse(Request.Form["date_of_birth"]);
            studentInfo.joining_date = DateTime.Parse(Request.Form["joining_date"]);

            if (studentInfo.name.Length == 0 || studentInfo.department_id.Length == 0 || studentInfo.email.Length == 0 || studentInfo.nationality.Length == 0)
            {
                errorMessage = "Enter data into all fields.";
                return;
            }

            // Calculate age
            TimeSpan age_at_joining = studentInfo.joining_date - studentInfo.date_of_birth;
            int years = (int)(age_at_joining.TotalDays / 365.25);

            // Validate age
            if (years < 18)
            {
                errorMessage = "Staff member must be at least 18 years old.";
                return;
            }

            // Validate joining date
            if (studentInfo.joining_date < studentInfo.date_of_birth || studentInfo.joining_date > DateTime.Today)
            {
                errorMessage = "Joining date is erroneous, please double check the dates.";
                return;
            }

            try
            {
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
                    if (!string.IsNullOrEmpty(studentInfo.ImageBase64))
                    {
                        // Use the existing image from the database
                        imageData = Convert.FromBase64String(studentInfo.ImageBase64);
                    }
                    else
                    {
                        // No image selected and no existing image, set imageData to null or an appropriate default value
                        imageData = null; // You can modify this line to use an appropriate default image if needed
                    }
                }

                // SQL code to update student info and image in the database
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Student " +
                                 "SET student_name = @student_name, department_id=@department_id, email = @email, nationality = @nationality, date_of_birth=@date_of_birth, joining_date=@joining_date";

                    if (imageData != null)
                    {
                        sql += ", image = @image";
                    }

                    sql += " WHERE student_id = @student_id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@student_id", studentInfo.id);
                        command.Parameters.AddWithValue("@student_name", studentInfo.name);
                        command.Parameters.AddWithValue("@department_id", studentInfo.department_id);
                        command.Parameters.AddWithValue("@email", studentInfo.email);
                        command.Parameters.AddWithValue("@nationality", studentInfo.nationality);
                        command.Parameters.AddWithValue("@date_of_birth", studentInfo.date_of_birth);
                        command.Parameters.AddWithValue("@joining_date", studentInfo.joining_date);

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

            studentInfo.name = "";
            studentInfo.email = "";
            studentInfo.department_id = "";
            studentInfo.nationality = "";
            studentInfo.date_of_birth = DateTime.Today;
            studentInfo.joining_date = DateTime.Today;
            successMessage = "Data Successfully Updated in the Database";

            Response.Redirect("/Students/Index");
        }


    }
}
