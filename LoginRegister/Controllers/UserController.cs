using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LoginRegister.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginRegister.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult UserHome()
        {
            TransferDetails transferDetails = new TransferDetails();
            using (SqlConnection connectionString = new SqlConnection(UserDataAccessLayer.GetConnectionString()))
            {
                SqlCommand commandBalance = new SqlCommand("select dbo.ValidateUserEmail(@address)", connectionString);
                commandBalance.Parameters.Add("@address", SqlDbType.VarChar);
                commandBalance.Parameters["@address"].Value = LoginController.email;
                connectionString.Open();
                transferDetails.Balance = (decimal)commandBalance.ExecuteScalar();
                connectionString.Close();
            }
            ViewData["Balance"] = transferDetails.Balance;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("UserLogin", "Login");
        }

        public IActionResult UserBuy(BuyCoins buy)
        {
            string BankAccount;
            decimal Amount;
            if (buy.Amount != null)
            {
                BankAccount = buy.BankAccount;
                Amount = decimal.Parse(buy.Amount);
            }
            return View();
        }
    }
}
