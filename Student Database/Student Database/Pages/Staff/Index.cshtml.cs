using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Student_Database.Pages.Staff
{
    public class IndexModel : PageModel
    {
        public List<StaffInfo> listStaff = new List<StaffInfo>();

        public void OnGet(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve the earliest admitted staff
                    string earliestAdmissionSql = "SELECT MIN(joining_date) FROM Staff";
                    using (SqlCommand earliestAdmissionCommand = new SqlCommand(earliestAdmissionSql, connection))
                    {
                        object earliestAdmissionObj = earliestAdmissionCommand.ExecuteScalar();
                        DateTime earliestAdmissionDate = earliestAdmissionObj is DBNull ? DateTime.MinValue : (DateTime)earliestAdmissionObj;

                        // Use the earliest admission date as the default "from" date if not specified
                        DateTime defaultFromDate = fromDate ?? earliestAdmissionDate;

                        // Use the current date as the default "to" date if not specified
                        DateTime defaultToDate = toDate ?? DateTime.Now;

                        // Set the default values in the date selection fields
                        ViewData["FromDate"] = defaultFromDate.ToString("yyyy-MM-dd");
                        ViewData["ToDate"] = defaultToDate.ToString("yyyy-MM-dd");

                        string sql = "SELECT * FROM Staff WHERE joining_date >= @FromDate AND joining_date <= @ToDate";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@FromDate", defaultFromDate);
                            command.Parameters.AddWithValue("@ToDate", defaultToDate);

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
                                    staffInfo.date_of_birth = reader.GetDateTime(7);
                                    staffInfo.joining_date = reader.GetDateTime(8);
                                    staffInfo.salary = reader.GetDecimal(9);
                                    listStaff.Add(staffInfo);
                                }
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

        // New fields
        public DateTime date_of_birth { get; set; }
        public DateTime joining_date { get; set; }
        public decimal salary { get; set; }
        public string department_name { get; set; }
    }
}
