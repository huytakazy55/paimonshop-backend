using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaimonShopApi.Data;
using PaimonShopApi.Models;
namespace PaimonShopApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext context;

        public AuthController(DataContext context)
        {
            this.context = context;
        }

        [HttpPost("signup")]
        public ActionResult Create(SignupRequest signupRequest)
        {
           User user = new User();
           var existUser = context.Users.FirstOrDefault(x => x.UserName == signupRequest.userName);
           if(existUser != null) return StatusCode(403, "Username already exists"); //User không đặt trùng nhau
           if (signupRequest != null) {
                user.UserName = signupRequest.userName;
                user.Password = BCrypt.Net.BCrypt.HashPassword(signupRequest.password);
                // user.Password = signupRequest.password;
                user.NickName = signupRequest.nickName;
                context.Users.Add(user);
                context.SaveChanges();
                return Ok("Success");
           } else {
                return Unauthorized("Input data incorrect!");
           }
        }

        [HttpPost("signin")]
        public ActionResult Signin(SigninRequest signinRequest)
        {
            var user = context.Users.FirstOrDefault(x => x.UserName == signinRequest.userName);
            if(user == null) {
                return Unauthorized("Username not esist !!");
            }
            if (!BCrypt.Net.BCrypt.Verify(signinRequest.password, user.Password)) {
                return Unauthorized("Invalid Password!");
            }
            else {
                    var claim = new List<Claim>{
                        new Claim(ClaimTypes.Name, user.UserName),
                        // new Claim(ClaimTypes.Role, user.role),
                    };
                    var claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

                    var cookie = HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties{
                            ExpiresUtc = DateTime.UtcNow.AddHours(1),
                            IsPersistent = true,
                            AllowRefresh = true,
                            RedirectUri = "/home"
                        }
                    );
                    return Ok(cookie);
            }
        }

        [HttpGet("/profile")]
        [Authorize]
        public ActionResult Profile() {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            // var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var user = context.Users.FirstOrDefault(x => x.UserName == userName);
            return Ok(user.NickName);
        }

        [HttpPost("/logout")]
        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Ok("Xong");
        }

    }
}