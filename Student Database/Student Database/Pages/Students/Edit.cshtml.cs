using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using System.Data.SqlClient;

namespace Student_Database.Pages.Students
{
    public class EditModel : PageModel
    {
        public StudentInfo studentInfo= new StudentInfo();
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            String student_id = Request.Query["student_id"];

            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Student WHERE student_id=@student_id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@student_id",student_id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentInfo.id = "" + reader.GetInt32(0);
                                studentInfo.name = reader.GetString(1);
                                studentInfo.department_id = "" + reader.GetInt32(2);
                                studentInfo.nationality = reader.GetString(3);
                                studentInfo.email = reader.GetString(4);
                                studentInfo.created_at = reader.GetDateTime(5).ToString();
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
                                departmentInfo.department_id = reader.GetInt32(0);
                                departmentInfo.department_name = reader.GetString(1);
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

        public void OnPost() 
        {
            studentInfo.id = Request.Form["student_id"];
            studentInfo.name = Request.Form["name"];
            studentInfo.email = Request.Form["email"];
            studentInfo.department_id = Request.Form["department"];
            studentInfo.nationality = Request.Form["nationality"];
            if (studentInfo.name.Length == 0 || studentInfo.email.Length == 0 || studentInfo.department_id.Length == 0 || studentInfo.nationality.Length == 0)
            {
                errorMessage = "Enter Data into all Fields";
                return;
            }

            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Student " +
                                "SET student_name=@student_name, department_id=@department_id, nationality=@nationality, email=@email " +
                                "WHERE student_id=@student_id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@student_id", studentInfo.id);
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

            Response.Redirect("/Students/Index");
        }
    }
}
