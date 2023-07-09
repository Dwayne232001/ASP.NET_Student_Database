using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using Student_Database.Pages.Students;
using System.Data.SqlClient;

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
                    string maxIdSql = "SELECT MAX(staff_id) FROM Staff";
                    int maxId;
                    using (SqlCommand maxIdCommand = new SqlCommand(maxIdSql, connection))
                    {
                        maxId = (int)maxIdCommand.ExecuteScalar();
                    }
                    int newStaffId = maxId + 1;
                    string sql = "INSERT INTO Staff " +
                                "(staff_id, staff_name, department_id, nationality, email) " +
                                "VALUES (@staff_id, @staff_name, @department_id, @nationality, @email)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@staff_id", newStaffId);
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

            staffInfo.name = "";
            staffInfo.email = "";
            staffInfo.department_id = "";
            staffInfo.nationality = "";
            successMessage = "Data Successfully Entered into the DataBase";

            Response.Redirect("/Staff/Index");
        }
    }
}