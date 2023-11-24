using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace Ikimina.Pages.User
{
    public class LoginModel : PageModel
    {
        public string errorMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            string email = Request.Form["email"];
            string password = Request.Form["password"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                errorMessage = "Email and password are required";
                return;
            }

            // Validate the user by checking the hashed password and role in the database
            try
            {
                string conString = "Data Source=DESKTOP-SIGEG94;Initial Catalog=IkiminaCyacu;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT Password, Role FROM User_table WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string hashedPassword = reader.GetString(0);
                                string role = reader.GetString(1);

                                if (PasswordHasher.VerifyPassword(password, hashedPassword))
                                {
                                    // Authentication successful
                                    if (role == "user")
                                    {
                                        TempData["Message"] = "User logged in";
                                        Response.Redirect("/User/UserDashboard");
                                    }
                                    else if (role == "manager")
                                    {
                                        TempData["Message"] = "Manager logged in";
                                        Response.Redirect("/User/ManagerDashboard");
                                    }
                                }
                                else
                                {
                                    errorMessage = "Invalid email or password";
                                }
                            }
                            else
                            {
                                errorMessage = "User not found";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
