using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using System.Data.SqlClient;

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
                            departmentInfo.department_id = reader.GetInt32(0);
                            departmentInfo.department_name = reader.GetString(1);
                            listDepartments.Add(departmentInfo);
                        }
                    }
                }
            }
        }

        public void OnPost() 
        { 
            studentInfo.name = Request.Form["name"];
            studentInfo.email = Request.Form["email"];
            studentInfo.department_id = Request.Form["department"];
            studentInfo.nationality = Request.Form["nationality"];

            if(studentInfo.name.Length == 0 || studentInfo.email.Length == 0 || studentInfo.department_id.Length == 0 || studentInfo.nationality.Length == 0)
            {
                errorMessage = "Enter Data into all Fields";
                return;
            }

            // Save the data into the database
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Retrieve the highest student_id from the Student table
                    string maxIdSql = "SELECT MAX(student_id) FROM Student";
                    int maxId;
                    using (SqlCommand maxIdCommand = new SqlCommand(maxIdSql, connection))
                    {
                        maxId = (int)maxIdCommand.ExecuteScalar();
                    }
                    // Increment the highest student_id to get the new student_id
                    int newStudentId = maxId + 1;
                    string sql = "INSERT INTO Student " +
                                "(student_id, student_name, department_id, nationality, email) " +
                                "VALUES (@student_id, @student_name, @department_id, @nationality, @email)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@student_id", newStudentId);
                        command.Parameters.AddWithValue("@student_name", studentInfo.name);
                        command.Parameters.AddWithValue("@email", studentInfo.email);
                        command.Parameters.AddWithValue("@department_id", studentInfo.department_id);
                        command.Parameters.AddWithValue("@nationality", studentInfo.nationality);
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
            successMessage = "Data Successfully Entered into the DataBase";

            Response.Redirect("/Students/Index");
        }
    }

    public class DepartmentInfo
    {
        public int department_id { get; set; }
        public string department_name { get; set; }
    }

}
