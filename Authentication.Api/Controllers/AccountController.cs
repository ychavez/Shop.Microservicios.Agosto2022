

using Authentication.Api.DTO;
using Authentication.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ShopUser> userManager;
        private readonly SignInManager<ShopUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(IConfiguration configuration, 
            UserManager<ShopUser> userManager, 
            SignInManager<ShopUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        private async Task<bool> UserExists(string username)
            => await userManager.Users
                   .AnyAsync(u => u.UserName == username);

        private async Task<string> GetToken(ShopUser user)
        {
            var now = DateTime.UtcNow;
            var key = configuration.GetValue<string>("Identity:Key");

            var claims = new List<Claim>
            {
             new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
             new Claim(JwtRegisteredClaimNames.Jti,user.Id),
             new Claim(ClaimTypes.GroupSid,user.Tenant.ToString()),
             new Claim(JwtRegisteredClaimNames.Iat,now.ToUniversalTime().ToString(),ClaimValueTypes.Integer64),
             new Claim(JwtRegisteredClaimNames.Email,user.Email)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var roles = await userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials 
                = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)

            };

            var encodedJwt = new JwtSecurityTokenHandler();

            var token = encodedJwt.CreateToken(tokenDescriptor);

            return encodedJwt.WriteToken(token);

        }

     
        
        [HttpPost("Roles")]
        public async Task<ActionResult> CreateRole(string role)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            return NoContent();
        }

        [HttpPost("AddUserToRol")]
        public async Task<ActionResult> AddUserToRol(string userName, string role)
        {
            var user = await userManager.FindByNameAsync(userName);
            await userManager.AddToRoleAsync(user, role);
            return NoContent();
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            if (!await UserExists(loginDTO.UserName.ToLower()))
                return Unauthorized();

            var user = await userManager.Users.SingleAsync(x =>
                x.UserName.ToLower() == loginDTO.UserName.ToLower());

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if (!result.Succeeded)
                return Unauthorized();

            return Ok(new UserDTO
            {
                UserName = user.UserName,
                Token = await GetToken(user)
            });

        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName.ToLower()))
                return BadRequest("User already exists");

            var user = new ShopUser()
            {
                UserName = registerDTO.UserName.ToLower(),
                Tenant = registerDTO.Tenant
            };

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return new UserDTO
            {
                UserName = registerDTO.UserName,
                Token = await GetToken(user)
            };

        }

    }
}
