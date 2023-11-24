using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Ikimina.Pages.User
{
    public class CreateModel : PageModel
    {
        public UserInfo user = new UserInfo();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
        }
        public void OnPost()
        {
            UserInfo user = new UserInfo();
            user.Username = Request.Form["username"]; // Assuming you have a form field for username
            user.Email = Request.Form["email"];
            user.Password = Request.Form["password"]; // Assuming you have a form field for password
            user.RegistrationDate = DateTime.Now; // You may need to adjust this based on your requirements
            user.Role = Request.Form["role"]; // Assuming you have a form field for role

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Role))
            {
                errorMessage = "All fields are required";
                return;
            }

            // Save the data
            try
            {
                string conString = "Data Source=DESKTOP-SIGEG94;Initial Catalog=IkiminaCyacu;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "INSERT INTO User_table(Username, Email, Password, RegistrationDate, Role) VALUES(@Username, @Email, @Password, @RegistrationDate, @Role)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        // Use parameterized queries to prevent SQL injection
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Password", PasswordHasher.HashPassword(user.Password)); // Hash the password
                        cmd.Parameters.AddWithValue("@RegistrationDate", user.RegistrationDate);
                        cmd.Parameters.AddWithValue("@Role", user.Role);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            // Clear input fields after successful insertion
            user.Username = "";
            user.Email = "";
            user.Password = "";
            user.Role = "";

            successMessage = "New User Added with Success";
            Response.Redirect("/User/Index"); // Assuming you have an index page for users
        }

    }
}
