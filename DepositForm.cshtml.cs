using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Ikimina.Pages.User
{
    public class DepositFormModel : PageModel
    {
        public TransactionInfo transaction = new TransactionInfo();
        public string errorMessage = "";
        public string successMessage = "";

        [BindProperty]
        public int UserID { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }

        [BindProperty]
        public string TransactionType { get; set; }

        [BindProperty]
        public DateTime TransactionDate { get; set; }

        // Add Status property
        [BindProperty]
        public string Status { get; set; } = "Pending";

        public void OnGet()
        {
            // Initialize default values or perform any other necessary logic
        }

        public IActionResult OnPost()
        {
            // Validate user input (optional)
            if (Amount <= 0)
            {
                errorMessage = "Amount must be greater than 0";
                return Page();
            }

            // Set values for the transaction
            transaction.UserID = UserID;
            transaction.Amount = Amount;
            transaction.TransactionType = TransactionType;
            transaction.TransactionDate = DateTime.Now;
            // Set the Status property
            transaction.Status = Status;

            // Insert the transaction into the database
            try
            {
                string conString = "Data Source=DESKTOP-SIGEG94;Initial Catalog=IkiminaCyacu;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "INSERT INTO Transactions(UserID, Amount, TransactionType, TransactionDate, Status) VALUES(@UserID, @Amount, @TransactionType, @TransactionDate, @Status)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        // Use parameterized queries to prevent SQL injection
                        cmd.Parameters.AddWithValue("@UserID", transaction.UserID);
                        cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                        cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                        cmd.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                        cmd.Parameters.AddWithValue("@Status", transaction.Status);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }

            // Clear input fields after a successful deposit
            UserID = 0;
            Amount = 0;
            TransactionType = ""; // Set it to the default value if needed

            successMessage = "Deposit Transaction Added with Success";

            // Redirect to the ViewDepositsWithdrawals page
            return RedirectToPage("/User/ViewDepositsWithdrawals");
        }
    }
}
