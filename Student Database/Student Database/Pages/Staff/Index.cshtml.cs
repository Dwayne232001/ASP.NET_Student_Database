using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Student_Database.Pages.Staff
{
    public class IndexModel : PageModel
    {
        public List<StaffInfo> listStaff = new List<StaffInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Staff";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StaffInfo staffInfo = new StaffInfo();
                                staffInfo.id = "" + reader.GetInt32(0);
                                staffInfo.name = reader.GetString(1);
                                staffInfo.department_id = "" + reader.GetInt32(2);
                                staffInfo.nationality = reader.GetString(3);
                                staffInfo.email = reader.GetString(4);
                                staffInfo.created_at = reader.GetDateTime(5).ToString();
                                listStaff.Add(staffInfo);
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
    }

    public class StaffInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string department_id { get; set; }
        public string nationality { get; set; }
        public string email { get; set; }
        public string created_at { get; set; }
        public IFormFile image { get; set; }

        public string ImageBase64 { get; set; }
    }
}
