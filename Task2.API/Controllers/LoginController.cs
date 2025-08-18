using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Task2.Data.BOL;
using Task2.Data.DTO;

namespace Task2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppSettings _appsettings;
        public LoginController(IOptions<AppSettings> appsetting)
        {
            _appsettings = appsetting.Value;
            
        }

        [HttpGet("Login")]
        public ActionResult GetLogin(string email, string pass)
        {
            try
            {
                var appSettings = _appsettings;
                email = email?.Trim();
                if ((BCrypt.Net.BCrypt.Verify(email, appSettings.defaultUsername)) && (BCrypt.Net.BCrypt.Verify(pass, appSettings.defaultPassword)))
                {
                    #region DefaultAccount
                    UserTBL defaultAccount = new UserTBL
                    {
                        userEmail = "idari@altis.com.tr",
                        firstName = "Altis",
                        lastName = "Teknoloji",
                        userId = -1,
                        telephone = "05323653896"
                    };

                    return Ok(new UserAuth
                    {
                        token = GenerateToken(defaultAccount),
                        firstName = defaultAccount.firstName,
                        lastName = defaultAccount.lastName,
                        userEmail = defaultAccount.userEmail,
                        userId = defaultAccount.userId,
                    });
                    #endregion 
                }

                return BadRequest("Username or Password is Incorrect.");
            }
            catch (Exception ex)
            {
                return StatusCode(450, ex.ToString());
            }
        }


        private string GenerateToken(UserTBL account)
        {
            try
            {
                List<Claim> claimList = new List<Claim>();
                claimList.Add(new Claim("email", account.userEmail));
                claimList.Add(new Claim("userId", account.userId.ToString()));
                claimList.Add(new Claim("displayName", account.firstName + " " + account.lastName));

                var claimsIdentity = new ClaimsIdentity(claimList, CookieAuthenticationDefaults.AuthenticationScheme);

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_appsettings.jwtToken));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);


                var token = new JwtSecurityToken(
                    claims: claimList,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: cred
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.WriteToken(token);

                return "Bearer " + jwt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
