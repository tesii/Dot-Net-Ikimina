using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using BCrypt.Net;

namespace Ikimina.Pages.User
{
    public class IndexModel : PageModel
    {
        public List<UserInfo> listUsers = new List<UserInfo>();

        public void OnGet()
        {
            listUsers.Clear();
            try
            {
                String conString = "Data Source=DESKTOP-SIGEG94;Initial Catalog=IkiminaCyacu;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    String sqlQuery = "SELECT * FROM User_table"; // Updated query to retrieve users
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo user = new UserInfo(); // Create an instance of UserInfo
                                user.UserID = reader.GetInt32(0); // Assuming UserID is the first column
                                user.Username = reader.GetString(1);
                                user.Email = reader.GetString(2);

                                // Storing hashed passwords
                                user.Password = reader.GetString(3);

                                user.RegistrationDate = reader.GetDateTime(4);
                                user.Role = reader.GetString(5);
                                listUsers.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
            }
        }
    }

    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Role { get; set; }
    }

    public static class PasswordHasher
    {
        // Hashes a password using BCrypt
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verifies a password against a hashed password
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
