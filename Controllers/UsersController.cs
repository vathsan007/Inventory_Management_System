using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebapiProject.Authentication;
using WebapiProject.Models;
using WebapiProject.Repository;

namespace WebapiProject.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuth _authService;
        public static List<User> lstuser = new List<User>
        {
            new User{UserId=5,Name="React",Username="admin",Password="admin123",Email="admin@gmail.com",Phone="1234567890",Address="Office",Role="Admin"}
        };

        public UsersController(IUserRepository userRepository, IAuth authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_userRepository.GetUserByUsername(user.Username) != null)
            {
                return BadRequest("Username already exists.");
            }

            _userRepository.AddUser(user);
            return Ok("User registered successfully");
        }

        //[Authorize(Roles = "User,Admin")]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginCredentials loginRequest)
        {
            try
            {
                if (_userRepository.ValidateUserCredentials(loginRequest.Username, loginRequest.Password))
                {
                    var user = _userRepository.GetUserByUsername(loginRequest.Username);
                    var token = _authService.GenerateToken(user);
                    Console.WriteLine($"Generated Token: {token}");
                    return Ok(new { Token = token });
                }
            }
            catch (System.Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return Unauthorized("Invalid credentials");
        }


        [AllowAnonymous]
        [HttpPost("authentication")]
        public IActionResult Authentication([FromBody] User user)
        {
            var token = _authService.GenerateToken(user);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("getUser/{username}")]
        public IActionResult GetUser(string username)
        {
            var user = _userRepository.GetUserByUsername(username);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("User not found");
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_userRepository.GetUserById(id));
        }
    }
}
