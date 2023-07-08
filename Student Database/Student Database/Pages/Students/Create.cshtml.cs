using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student_Database.Pages.Clients;

namespace Student_Database.Pages.Students
{
    public class CreateModel : PageModel
    {
        public StudentInfo studentInfo = new StudentInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
        }

        public void OnPost() 
        { 
            studentInfo.name = Request.Form["name"];
            studentInfo.email = Request.Form["email"];
            studentInfo.department_id = Request.Form["department"];
            studentInfo.nationality = Request.Form["nationality"];

            if(studentInfo.name.Length != 0 || studentInfo.email.Length != 0 || studentInfo.department_id.Length != 0 || studentInfo.nationality.Length != 0)
            {
                errorMessage = "Enter Data into all Fields";
                return;
            }

            // Save the data into the database
            studentInfo.name = "";
            studentInfo.email = "";
            studentInfo.department_id = "";
            studentInfo.nationality = "";
            successMessage = "Data Successfully Entered into the DataBase";
        }
    }
}
