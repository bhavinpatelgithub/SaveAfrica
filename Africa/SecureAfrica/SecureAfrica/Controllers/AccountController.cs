using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureAfrica.DataModel;
using SecureAfrica.Models;

namespace SecureAfrica.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDataModels registerDataModels)
        {

            var user = new ApplicationUser
            {
                UserName = registerDataModels.PhoneNumber,
                Email = registerDataModels.Email,
                PhoneNumber = registerDataModels.PhoneNumber,
                Name = registerDataModels.Name,
                Country = registerDataModels.Country,
                InternationalPrefix = registerDataModels.InternationalPrefix,
                Address = registerDataModels.Address,
                CoordinateX = registerDataModels.CoordinateX,
                CoordinateY = registerDataModels.CoordinateY,
                Source = registerDataModels.Source,
                PushTokenId = registerDataModels.PushTokenId

            };

            var result = await userManager.CreateAsync(user, registerDataModels.Password);
            if (result.Succeeded)
            {
                if (user.Source == "Android" || user.Source == "iOS")
                {
                    var role = await roleManager.FindByNameAsync("User");
                    if (role != null)
                    {
                        IdentityResult roleresult = null;
                        roleresult = await userManager.AddToRoleAsync(user, role.Name);
                        if (!roleresult.Succeeded)
                        {
                            return BadRequest("New user Created But Role has been not assigned !");
                        }
                    }
                    else
                    {
                        return BadRequest("New user Created But Role not found !");
                    }
                }
                if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                {
                    return Ok("New user Created!");
                }
                else
                {
                    LoginDataModel loginDataModel = new LoginDataModel
                    {
                        UserName = user.UserName,
                        Password = registerDataModels.Password,
                        RememberMe = false
                    };
                   var returnResult =  await Login(loginDataModel);
                    //  await signInManager.SignInAsync(user, isPersistent: true);
                      return Ok(returnResult);
                }
            }
            List<string> errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }
            return BadRequest(new { message = errors });

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDataModel loginDataModel)
        {
            var result = await signInManager.PasswordSignInAsync(loginDataModel.UserName, loginDataModel.Password, loginDataModel.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(loginDataModel.UserName);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
                var signinkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecurityKey"));
                var tocken = new JwtSecurityToken(
                   issuer: "http://saveafrica.com",
                   audience: "http://saveafrica.com",
                   expires: DateTime.Now.AddHours(1),
                   claims: claims,
                   signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signinkey, SecurityAlgorithms.HmacSha256)
                );

                // return Ok(user);
                return Ok(new
                {
                    authonticationTocken = new JwtSecurityTokenHandler().WriteToken(tocken),
                    expiration = tocken.ValidTo,
                    userId = user.Id
                });
            }
            else
            {
                return BadRequest("Invalid Username Or Passwrord!");
            }
        }


        [HttpPost("createrole")]
        public async Task<IActionResult> CreateRole(CreateRoleDataModel createRoleDataModel)
        {
            IdentityRole role = new IdentityRole(createRoleDataModel.RoleName);
            IdentityResult result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok(role);
            }
            return BadRequest("Role Already Existed!");
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok("Logout Successfull !");
        }
    }
}