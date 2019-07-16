using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LoginRegister.Models
{
    public class UserDataAccessLayer
    {
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Reads ConnectionString from appsettings.json file  
        /// </summary>
        public static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            string connectionString = Configuration["ConnectionStrings:myConString"];

            return connectionString;

        }

        private readonly string connectionString = GetConnectionString(); // Connection string

        /// <summary>
        /// Registers a new user 
        /// </summary>
        /// <param name="user"> User</param>
        public string RegisterUser(UserDetails user)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                if (IsValidEmail(user.Email))
                {
                    SqlCommand commandRegister = new SqlCommand("RegisterUser", connection);
                    commandRegister.CommandType = CommandType.StoredProcedure;

                    commandRegister.Parameters.AddWithValue("@FirstName", user.FirstName);
                    commandRegister.Parameters.AddWithValue("@LastName", user.LastName);
                    commandRegister.Parameters.AddWithValue("@Email", user.Email);
                    commandRegister.Parameters.AddWithValue("@UserPassword", ComputeSha256Hash(user.Password));
                    commandRegister.Parameters.AddWithValue("@Balance", 0);
                    connection.Open();
                    string result = commandRegister.ExecuteScalar().ToString();
                    return result;
                }
                return null;
            }
        }

        /// <summary>
        /// Validates the login   
        /// </summary>
        /// <param name="user"> User </param>
        public string ValidateLogin(UserDetails user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand commandUser = new SqlCommand("ValidateUserLogin", connection);
                commandUser.CommandType = CommandType.StoredProcedure;

                commandUser.Parameters.AddWithValue("@LoginEmail", user.Email);
                commandUser.Parameters.AddWithValue("@LoginPassword", ComputeSha256Hash(user.Password));

                connection.Open();
                string result = commandUser.ExecuteScalar().ToString();

                return result;
            }
        }

        /// <summary>
        /// Validate the user
        /// </summary>
        /// <param name="email"> User email </param>
        public string ValidateUserEmail(string email)
        {
            if (IsValidEmail(email))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand commandUser = new SqlCommand("ValidateUser", connection);
                    commandUser.CommandType = CommandType.StoredProcedure;

                    commandUser.Parameters.AddWithValue("@LoginEmail", email);
                    connection.Open();
                    string result = commandUser.ExecuteScalar().ToString();

                    return result;
                }
            }
            else
            {
                return "Failed";
            }
        }

        /// <summary>
        /// Hashing a password 
        /// </summary>
        /// <param name="rawData"> Password </param>
        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Validates the Email
        /// </summary>
        /// <param name="email"> Email</param>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
