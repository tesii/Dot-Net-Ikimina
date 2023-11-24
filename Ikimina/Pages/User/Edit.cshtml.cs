using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace Ikimina.Pages.User
{
    public class EditModel : PageModel
    {
        public UserInfo user = new UserInfo(); // Update to your actual user model
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            // Assuming you have the UserID in the query string
            if (!int.TryParse(Request.Query["UserID"], out int userID))
            {
                // Handle invalid or missing UserID parameter
                errorMessage = "Invalid or missing UserID parameter.";
                return;
            }

            try
            {
                string conString = "Data Source=DESKTOP-SIGEG94;Initial Catalog=IkiminaCyacu;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM User_table WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user.UserID = reader.GetInt32(0);
                                user.Username = reader.GetString(1);
                                user.Email = reader.GetString(2);
                                // Assuming other properties of UserInfo
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }

        public void OnPost()
        {
            if (!int.TryParse(Request.Form["UserID"], out int userID))
            {
                // Handle invalid or missing UserID parameter
                errorMessage = "Invalid or missing UserID parameter.";
                return;
            }

            user.UserID = userID;
            user.Username = Request.Form["Username"];
            user.Email = Request.Form["Email"];
            // Assuming other properties of UserInfo
            // ...

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email))
            {
                errorMessage = "Username and Email are required";
                return;
            }

            // Update the user data
            try
            {
                string conString = "Data Source=DESKTOP-SIGEG94;Initial Catalog=IkiminaCyacu;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "UPDATE User_table SET Username = @Username, Email = @Email WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        // Add parameters for other properties if needed
                        // ...

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            user.Username = "";
            user.Email = "";
            // Reset other properties if needed
            // ...

            successMessage = "User Updated with success";
            Response.Redirect("/User/Index"); // Update to your actual index page
        }
    }
}
