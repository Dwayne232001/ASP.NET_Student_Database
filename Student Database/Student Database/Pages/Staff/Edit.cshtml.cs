using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            String staff_id = Request.Query["staff_id"];

            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Staff WHERE staff_id=@staff_id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@staff_id", staff_id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                staffInfo.id = "" + reader.GetInt32(0);
                                staffInfo.name = reader.GetString(1);
                                staffInfo.department_id = "" + reader.GetInt32(2);
                                staffInfo.nationality = reader.GetString(3);
                                staffInfo.email = reader.GetString(4);
                                staffInfo.created_at = reader.GetDateTime(5).ToString();
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
            staffInfo.id = Request.Form["staff_id"];
            staffInfo.name = Request.Form["name"];
            staffInfo.email = Request.Form["email"];
            staffInfo.department_id = Request.Form["department"];
            staffInfo.nationality = Request.Form["nationality"];
            if (staffInfo.name.Length == 0 || staffInfo.email.Length == 0 || staffInfo.department_id.Length == 0 || staffInfo.nationality.Length == 0)
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
                    string sql = "UPDATE Staff " +
                                "SET staff_name=@staff_name, department_id=@department_id, nationality=@nationality, email=@email " +
                                "WHERE staff_id=@staff_id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@staff_id", staffInfo.id);
                        command.Parameters.AddWithValue("@staff_name", staffInfo.name);
                        command.Parameters.AddWithValue("@email", staffInfo.email);
                        command.Parameters.AddWithValue("@department_id", staffInfo.department_id);
                        command.Parameters.AddWithValue("@nationality", staffInfo.nationality);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Staff/Index");
        }
    }
}