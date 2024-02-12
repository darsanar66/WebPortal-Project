using FluentValidation;
using Laya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Laya.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IValidator<Registration> _registrationValidator;
        private readonly IValidator<Login> _loginValidator;

        public RegistrationController(IConfiguration configuration, IValidator<Registration> registrationValidator, IValidator<Login> loginValidator)
        {
            _configuration = configuration;
            _registrationValidator = registrationValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(Registration registration)
        {
            var validationResult = _registrationValidator.Validate(registration);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Check if the user with the same UserName or Email already exists
            if (UserExists(registration.UserName) || EmailExists(registration.Email))
            {
                return BadRequest("User with the same UserName or Email already registered");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "INSERT INTO registration (UserName, Password, Email) VALUES (@UserName, @Password, @Email)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", registration.UserName);
                    cmd.Parameters.AddWithValue("@Password", registration.Password);
                    cmd.Parameters.AddWithValue("@Email", registration.Email);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Registration Successful");
                    }
                    else
                    {
                        return BadRequest("Registration Failed");
                    }
                }
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(Login login)
        {
            var validationResult = _loginValidator.Validate(login);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "SELECT UserName FROM registration WHERE UserName = @UserName AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", login.UserName);
                    cmd.Parameters.AddWithValue("@Password", login.Password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        return Ok("Login Successful");
                    }
                    else
                    {
                        return Unauthorized("Invalid Login Credentials");
                    }
                }
            }
        }

        private bool UserExists(string userName)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM registration WHERE UserName = @UserName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);

                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        private bool EmailExists(string email)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = "SELECT COUNT(*) FROM registration WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }

    public class RegistrationValidator : AbstractValidator<Registration>
    {
        public RegistrationValidator()
        {
            RuleFor(registration => registration.UserName).MinimumLength(10).WithMessage("The UserName should be atleast 10 charecters.");
            RuleFor(registration => registration.Password).MinimumLength(6);
            RuleFor(registration => registration.Email).NotEmpty().EmailAddress();
        }
    }

    public class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator()
        {
            RuleFor(login => login.UserName).NotEmpty();
            RuleFor(login => login.Password).NotEmpty();
        }
    }
}






















// using FluentValidation;
// using Laya.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Data.SqlClient;
// using Microsoft.Extensions.Configuration;
// using System.Data;


// namespace Laya.Controllers
// {
    
//     [Route("api/[Controller]")]
//     [ApiController]
//     public class RegistrationController : ControllerBase
//     {
//         private readonly IConfiguration _configuration;
//         private readonly IValidator<Registration> _registrationValidator;

//         private readonly IValidator<Login> _loginValidator;

//          public RegistrationController(IConfiguration configuration, IValidator<Registration> registrationValidator, IValidator<Login> loginValidator)
    
//         {
            
//             _configuration = configuration;
//             _registrationValidator = registrationValidator;
//             _loginValidator = loginValidator;
//         }

//         [HttpPost]
//         [Route("Registration")]
//         public IActionResult Registration(Registration registration)
//         {
//             var validationResult = _registrationValidator.Validate(registration);
//             if (!validationResult.IsValid)
//             {
//                 return BadRequest(validationResult.Errors);
//             }

//             // Check if the user with the same UserName or Email already exists
//             if (UserExists(registration.UserName) || EmailExists(registration.Email))
//             {
//                 return BadRequest("User with the same UserName or Email already registered");
//             }

//             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//             {
//                 con.Open();

//                 string query = "INSERT INTO registration (UserName, Password, Email) VALUES (@UserName, @Password, @Email)";

//                 using (SqlCommand cmd = new SqlCommand(query, con))
//                 {
//                     cmd.Parameters.AddWithValue("@UserName", registration.UserName);
//                     cmd.Parameters.AddWithValue("@Password", registration.Password);
//                     cmd.Parameters.AddWithValue("@Email", registration.Email);
                   

//                     int rowsAffected = cmd.ExecuteNonQuery();

//                     if (rowsAffected > 0)
//                     {
//                         return Ok("Registration Successful");
//                     }
//                     else
//                     {
//                         return BadRequest("Registration Failed");
//                     }
//                 }
//             }
//         }

//         [HttpPost]
//         [Route("Login")]
//         public IActionResult Login(Login login)
//         {
//             var validationResult = _loginValidator.Validate(login);
//             if (!validationResult.IsValid)
//             {
//                 return BadRequest(validationResult.Errors);
//             }

//             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//             {
//                 con.Open();

//                 string query = "SELECT UserName FROM registration WHERE UserName = @UserName AND Password = @Password";

//                 using (SqlCommand cmd = new SqlCommand(query, con))
//                 {
//                     cmd.Parameters.AddWithValue("@UserName", login.UserName);
//                     cmd.Parameters.AddWithValue("@Password", login.Password);

//                     SqlDataReader reader = cmd.ExecuteReader();

//                     if (reader.HasRows)
//                     {
//                         return Ok("Login Successful");
//                     }
//                     else
//                     {
//                         return Unauthorized("Invalid Login Credentials");
//                     }

                    
//                 }
//             }
//         }

//         private bool UserExists(string userName)
//         {
//             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//             {
//                 con.Open();

//                 string query = "SELECT COUNT(*) FROM registration WHERE UserName = @UserName";

//                 using (SqlCommand cmd = new SqlCommand(query, con))
//                 {
//                     cmd.Parameters.AddWithValue("@UserName", userName);

//                     int count = (int)cmd.ExecuteScalar();

//                     return count > 0;
//                 }
//             }
//         }

//         private bool EmailExists(string email)
//         {
//             using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//             {
//                 con.Open();

//                 string query = "SELECT COUNT(*) FROM registration WHERE Email = @Email";

//                 using (SqlCommand cmd = new SqlCommand(query, con))
//                 {
//                     cmd.Parameters.AddWithValue("@Email", email);

//                     int count = (int)cmd.ExecuteScalar();

//                     return count > 0;
//                 }
//                 public class RegistrationValidator : AbstractValidator<Registration>
//                 {
//                     public class RegistrationValidator()
//                     {
//                         RuleFor(registration => registration.UserName).NotEmpty();
//                         RuleFor(registration => registration.Password).NotEmpty().MinimumLength(6);
//                         RuleFor(registration => registration.Email).NotEmpty().EmailAddress();
//                     }
//                     public class LoginValidator : AbstractValidator<Login>
//                     {
//                         public LoginValidator()
//                         {
//                              RuleFor(login => login.UserName).NotEmpty();
//                              RuleFor(login => login.Password).NotEmpty();
//                         }
//                     }
//                 }
//             }
//         }
//     }
// }