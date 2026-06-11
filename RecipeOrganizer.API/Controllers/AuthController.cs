using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeOrganizer.Domain.Entity;
using RecipeOrganizer.Domain.Services;

namespace RecipeOrganizer.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ValidateRegisterRequest(request))
                {
                    return BadRequest(new
                    {
                        Success = false
                    });
                }

                RegisterResponse response = await _authService.RegisterAsync(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        private bool ValidateRegisterRequest(RegisterRequest request)
        {

            if (request == null)
                return false;

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
                return false;

            return true;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ValidateLoginRequest(request, out List<string> errors))
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Errors = errors
                    });
                }

                LoginResponse response = await _authService.LoginAsync(request);

                return StatusCode(response.ResponseCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private bool ValidateLoginRequest(LoginRequest request, out List<string> errors)
        {
            errors = new();

            if (request == null)
            {
                errors.Add("Request cannot be null.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
                errors.Add("All Fields are required.");

            return !errors.Any();
        }

        [Authorize]
        [HttpGet]
        [Route("Profile")]
        public async Task<IActionResult> Profile()
        {
            string userName = User.FindFirst("userName")?.Value ?? string.Empty;

            UserProfileRequest request = new UserProfileRequest { UserNames = new List<string> { userName }};

            UserProfileResponse response = await _authService.GetUserProfileAsync(request);

            return StatusCode(response.ResponseCode, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            try
            {
                BaseResponse response = await _authService.AssignRoleAsync(request);

                return StatusCode(response.ResponseCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
