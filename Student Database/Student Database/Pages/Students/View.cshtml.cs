using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using Student_Database.Pages.Departments;
using System.Data.SqlClient;

namespace Student_Database.Pages.Students
{
    public class ViewModel : PageModel
    {
        public StudentInfo studentInfo = new StudentInfo();
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();
        public String errorMessage = "";
        public String successMessage = "";

        private string GetDepartmentNameById(string department_id)
        {
            foreach (DepartmentInfo departmentInfo in listDepartments)
            {
                if (departmentInfo.id == studentInfo.department_id)
                {
                    return departmentInfo.name;
                }
            }
            return "Unknown Department"; // Return a default value if the department_id is not found in the list
        }

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
                    studentInfo.department_name = GetDepartmentNameById(studentInfo.department_id);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception: " + x.ToString());
            }
        }
    }
}
