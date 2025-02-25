using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.DTO;
using RedMango_API.Utility;
using System.Net;

namespace RedMango_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _db;
        private ApiResponse _response;
        private string secretKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(AppDBContext db, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _response = new ApiResponse();
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerModel)
        {
            ApplicationUser userFromDB = _db.ApplicationUsers.FirstOrDefault(x=>x.UserName.ToLower() == registerModel.Username.ToLower());

            if (userFromDB != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }
            else
            {
                ApplicationUser newUser = new()
                {
                    UserName = registerModel.Username,
                    Email = registerModel.Username,
                    NormalizedEmail = registerModel.Username.ToUpper(),
                    Name = registerModel.Name,
                };
                try
                {
                    var result = await _userManager.CreateAsync(newUser, registerModel.Password);
                    if (result.Succeeded)
                    {
                        if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                        {
                            // Create roles in DB
                            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                        }
                        if (registerModel.Role.ToLower() == SD.Role_Admin)
                        {
                            await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                        }
                        _response.StatusCode = HttpStatusCode.OK;
                        _response.IsSuccess = true;
                        return Ok(_response);
                    }
                    else
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        _response.ErrorMessages.Add("Error while registering");
                        return BadRequest(_response);
                    }
                }
                catch (Exception)
                {

                }
                return null;
            } 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginModel)
        {
            ApplicationUser userFromDB = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginModel.Username.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(userFromDB, loginModel.Password);
            if (isValid == false)
            {
                _response.Result = new LoginResponseDTO();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            else
            {
                // generate JWT Token
                LoginResponseDTO loginResponse = new()
                {
                    Email = userFromDB.Email,
                    Token = "REPLACE WITH ACTUAL TOKEN ONCE GENERATED"
                };
                if (loginResponse.Email == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username or password is incorrect");
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = loginResponse;
                    return Ok(_response);
                }
            }
        }
    }
}
