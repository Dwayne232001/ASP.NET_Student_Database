using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using Student_Database.Pages.Departments;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace Student_Database.Pages.Students
{
    public class CreateModel : PageModel
    {
        public StudentInfo studentInfo = new StudentInfo();
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
            studentInfo.name = Request.Form["name"];
            studentInfo.email = Request.Form["email"];
            studentInfo.department_id = Request.Form["department"];
            studentInfo.nationality = Request.Form["nationality"];

            if (studentInfo.name.Length == 0 || studentInfo.email.Length == 0 || studentInfo.nationality.Length == 0 || image == null)
            {
                errorMessage = "Enter data into all fields and select an image.";
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
                // SQL code to insert student info and image into the database
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string maxIdSql = "SELECT MAX(student_id) FROM Student";
                    int? maxId = null; // Use nullable int to handle no rows scenario
                    using (SqlCommand maxIdCommand = new SqlCommand(maxIdSql, connection))
                    {
                        var result = maxIdCommand.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            maxId = Convert.ToInt32(result);
                        }
                    }
                    int newStudentId = maxId.HasValue ? maxId.Value + 1 : 1; // Set initial student_id to 1 if maxId is null
                    string sql = "INSERT INTO Student " +
                                 "(student_id, student_name, department_id,email, nationality, image) " +
                                 "VALUES (@student_id, @student_name, @department_id, @email, @nationality, @image)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@student_id", newStudentId);
                        command.Parameters.AddWithValue("@student_name", studentInfo.name);
                        command.Parameters.AddWithValue("@email", studentInfo.email);
                        command.Parameters.AddWithValue("@department_id", studentInfo.department_id);
                        command.Parameters.AddWithValue("@nationality", studentInfo.nationality);
                        command.Parameters.AddWithValue("@image", imageData);
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
            studentInfo.nationality = "";
            successMessage = "Data Successfully Entered into the Database";

            Response.Redirect("/Students/Index");
        }
    }
}
