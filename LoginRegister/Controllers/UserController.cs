﻿using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using LoginRegister.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebSocketSharp;

namespace LoginRegister.Controllers
{
    [Authorize]
    public class UserController : Controller
    {

        public static string url = $"ws://127.0.0.1:6001/Blockchain";

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UserHome(TransferDetails transferDetails)
        {
            ViewData["Balance"] = GetBalance();
            if (transferDetails.Amount != null)
            {
                var userData = new UserDataAccessLayer();
                var FromAddress = LoginController.email;
                var ToAddress = transferDetails.Address;
                var Amount = decimal.Parse(transferDetails.Amount);
                var Type = "transaction";
                var LoginStatus = userData.ValidateUserEmail(ToAddress);

                if (GetBalance() >= Amount && LoginStatus == "Success")
                {
                    using (WebSocket web = new WebSocket(url))
                    {
                        web.Connect();
                        web.Send(JsonConvert.SerializeObject(new { Type, FromAddress, ToAddress, Amount }));
                        Thread.Sleep(5000);
                        web.Close();
                    }
                }
                else
                {
                    TempData["UserEmailFailed"] = "The email you’ve entered doesn’t match any account";
                }
            }
            return View();            
        }

        private decimal GetBalance()
        {
            TransferDetails transferDetails = new TransferDetails();
            using (SqlConnection connection = new SqlConnection(UserDataAccessLayer.GetConnectionString()))
            {
                SqlCommand commandBalance = new SqlCommand("select dbo.ValidateUserEmail(@address)", connection);
                commandBalance.Parameters.Add("@address", SqlDbType.VarChar);
                commandBalance.Parameters["@address"].Value = LoginController.email;
                connection.Open();
                transferDetails.Balance = (decimal)commandBalance.ExecuteScalar();
            }
            return transferDetails.Balance;
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
                var FromAddress = string.Empty;
                var ToAddress = LoginController.email;
                var Type = "transaction";

                BankAccount = buy.BankAccount;
                Amount = decimal.Parse(buy.Amount);

                using (WebSocket web = new WebSocket(url))
                {
                    web.Connect();
                    web.Send(JsonConvert.SerializeObject(new { Type, FromAddress, ToAddress, Amount }));
                    Thread.Sleep(5000);
                    web.Close();
                }
            }
            return View();
        }
    }
}
