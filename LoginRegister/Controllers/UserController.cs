using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace RegisterLogin.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult UserHome()
        {
            LoginRegister.Models.TransferDetails transferDetails = new LoginRegister.Models.TransferDetails();
            using (SqlConnection con = new SqlConnection(LoginRegister.Models.UserDataAccessLayer.GetConnectionString()))
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
            return View();
           
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpPost]
        public IActionResult Transfer(string Address, string Amount)
        {
            System.Console.WriteLine($"{Address} {Amount}");
            return RedirectToAction("UserLogin", "Login");
        }
    }
}
