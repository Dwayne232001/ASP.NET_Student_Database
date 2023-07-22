using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using Student_Database.Pages.Departments;
using System.Data.SqlClient;

namespace Student_Database.Pages.Staff
{
    public class ViewModel : PageModel
    {
        public StaffInfo staffInfo = new StaffInfo();
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();
        public String errorMessage = "";
        public String successMessage = "";

        private string GetDepartmentNameById(string department_id)
        {
            foreach (DepartmentInfo departmentInfo in listDepartments)
            {
                if (departmentInfo.id == staffInfo.department_id)
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
                                departmentInfo.id = "" + reader.GetInt32(0);
                                departmentInfo.name = reader.GetString(1);
                                listDepartments.Add(departmentInfo);
                            }
                        }
                    }
                    staffInfo.department_name = GetDepartmentNameById(staffInfo.department_id);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception: " + x.ToString());
            }
        }
    }
}
