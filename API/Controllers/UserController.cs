using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using API.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly IMailer _mailer;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager,IOptions<ApplicationSettings> appSettings,IMailer mailer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _mailer = mailer;
        }

        // POST api/Register
        [HttpPost]
        [Route("Register")]
        public  async Task<Object> PostUser([FromBody] UserInterface userInterface)
        {
            var user = new User()
            {
                Email = userInterface.email,
                UserName = userInterface.email,
                firstName = userInterface.firstName,
                lastName = userInterface.lastName,
                mobileNumber = userInterface.mobileNumber
            };

            try {
                var result = await _userManager.CreateAsync(user, userInterface.password);
                return Ok(result);
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        // POST api/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInterface loginInterface)
        {
            var user = await _userManager.FindByEmailAsync(loginInterface.email);
            if(user != null && await _userManager.CheckPasswordAsync(user, loginInterface.password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserId",user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_SECRET)),SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
            {
                return BadRequest(new { message = "Username or password incorrect" });
            }
        }


        // Post api/Reset
        [HttpPost]
        [Route("Reset")]
        public async Task<IActionResult> Reset([FromBody] ResetInterface resetInterface) {
            var user = await _userManager.FindByEmailAsync(resetInterface.email);
            if(user == null)
            {
                return BadRequest(new { message = "Email not found!" });
            }

            var random = new Random();
            var code = random.Next(1, 400);
            user.resetPasswordCode = code.ToString();
            await _userManager.UpdateAsync(user);

            await _mailer.SendEmailAsync(resetInterface.email, "Reset Code", code.ToString());
            var success = "success";
            return Ok(new { success });
        }


        [HttpPost]
        [Route("Reset/Post")]
        public async Task<IActionResult> ResetPost([FromBody] ResetPostInterface resetPostInterface)
        {
            var user = await _userManager.FindByEmailAsync(resetPostInterface.email);
            if (user == null)
            {
                return BadRequest(new { message = "Email not found!" });
            }

            if(user.resetPasswordCode == resetPostInterface.code)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user,token,resetPostInterface.password);
                var success = "success";
                return Ok(new { success });
            }

            return BadRequest(new { message = "Invalid Code!" });
        }

        [HttpGet]
        [Route("TestMail")]
        public async Task<IActionResult> Test()
        {
            await _mailer.SendEmailAsync("pula0404@gmail.com", "testMail", "Testing Mail");
            var success = "";
            return Ok(new { success });
        }


    }
}
