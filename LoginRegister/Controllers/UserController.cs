using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoginRegister.Models;
using MagMan;

namespace LoginRegister.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult UserHome()
        {
            Models.TransferDetails transferDetails = new Models.TransferDetails();
            using (SqlConnection con = new SqlConnection(Models.UserDataAccessLayer.GetConnectionString()))
            {
                SqlCommand cmd1 = new SqlCommand("select dbo.ValidateUserEmail(@address)", con);
                cmd1.Parameters.Add("@address", SqlDbType.VarChar);
                cmd1.Parameters["@address"].Value = LoginController.email;
                con.Open();
                transferDetails.Balance = (decimal)cmd1.ExecuteScalar();
                con.Close();
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
                WebSocket ws = new WebSocket(url);
                ws.OnMessage += (sender, e) =>
                {

                };
            return View();
            }
        }
    }
}
