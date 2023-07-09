using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace Student_Database.Pages.Departments
{
    public class CreateModel : PageModel
    {
        public DepartmentInfo departmentInfo = new DepartmentInfo();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnPost()
        {
            departmentInfo.name = Request.Form["name"];

            if (departmentInfo.name.Length == 0)
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
                    // Retrieve the highest department_id from the Department table
                    string maxIdSql = "SELECT MAX(department_id) FROM Department";
                    int maxId;
                    using (SqlCommand maxIdCommand = new SqlCommand(maxIdSql, connection))
                    {
                        maxId = (int)maxIdCommand.ExecuteScalar();
                    }
                    // Increment the highest department_id to get the new department_id
                    int newDepartmentId = maxId + 1;
                    string sql = "INSERT INTO Department " +
                                "(department_id, department_name) " +
                                "VALUES (@department_id, @department_name)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@department_id", newDepartmentId);
                        command.Parameters.AddWithValue("@department_name", departmentInfo.name);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            departmentInfo.name = "";
            successMessage = "Data Successfully Entered into the DataBase";

            Response.Redirect("/Departments/Index");
        }
    }
}
