using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using AWSDeployDemo.Authenticate;
using System.Security.Claims;

namespace DeployDemo.Controllers
{
    [Authorize()]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IJwtAuth _auth;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IJwtAuth auth)
        {
            _logger = logger;
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var identity  = HttpContext.User.Identity as ClaimsIdentity;
            if(identity ==null)
            {
                return Unauthorized();
            }

            var claim = identity.FindFirst("MyRole").Value;
            if(claim!="Boss1")
                return Unauthorized();

            


            var rng = new Random();
            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray());
        }

        [AllowAnonymous]
        [HttpPost("authentication")]
        public IActionResult Authentication(UserCredentials userCred)
        {
            var token = _auth.Authenticate(userCred.Username,userCred.Password);
            if(token == null)
                return Unauthorized();
            return Ok(token);
        }
    }


    public class UserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }

}

