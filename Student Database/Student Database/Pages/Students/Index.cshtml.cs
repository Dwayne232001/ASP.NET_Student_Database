using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Student_Database.Pages.Clients
{
    public class IndexModel : PageModel
    {
        public List<StudentInfo> listStudents = new List<StudentInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Student";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StudentInfo studentInfo = new StudentInfo();
                                studentInfo.id = ""+reader.GetInt32(0);
                                studentInfo.name = reader.GetString(1);
                                studentInfo.department_id = "" + reader.GetInt32(2);
                                studentInfo.nationality = reader.GetString(3);
                                studentInfo.email = reader.GetString(4);
                                studentInfo.created_at = reader.GetDateTime(5).ToString();
                                listStudents.Add(studentInfo);
                            }
                        }
                    }
                }

                Console.WriteLine(listStudents[1].id);
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception: " + x.ToString());
            }
        }
    }

    public class StudentInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string department_id { get; set; }
        public string nationality { get; set; }
        public string email { get; set; }
        public string created_at { get; set; }
    }
}
