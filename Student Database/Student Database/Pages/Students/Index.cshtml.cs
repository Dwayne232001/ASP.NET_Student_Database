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

        public void OnGet(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=collegedata;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve the earliest admitted student
                    string earliestAdmissionSql = "SELECT MIN(joining_date) FROM Student";
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

                        string sql = "SELECT * FROM Student WHERE joining_date >= @FromDate AND joining_date <= @ToDate";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@FromDate", defaultFromDate);
                            command.Parameters.AddWithValue("@ToDate", defaultToDate);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    StudentInfo studentInfo = new StudentInfo();
                                    studentInfo.id = "" + reader.GetInt32(0);
                                    studentInfo.name = reader.GetString(1);
                                    studentInfo.department_id = "" + reader.GetInt32(2);
                                    studentInfo.nationality = reader.GetString(3);
                                    studentInfo.email = reader.GetString(4);
                                    studentInfo.created_at = reader.GetDateTime(5).ToString();
                                    studentInfo.date_of_birth = reader.GetDateTime(7);
                                    studentInfo.joining_date = reader.GetDateTime(8);
                                    listStudents.Add(studentInfo);
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

        public class StudentInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string department_id { get; set; }
        public string nationality { get; set; }
        public string email { get; set; }
        public string created_at { get; set; }
        public IFormFile image { get; set; }
        public string ImageBase64 { get; set; }
        public DateTime date_of_birth { get; set; }
        public DateTime joining_date { get; set; }
    }
}
