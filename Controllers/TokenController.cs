using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SwaggerTest.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SwaggerTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        [SwaggerOperation(Summary = "Cannot be tested in swagger", Description = "This request can only be tested successfully in postman")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(LoginRequest), Description = "Success")]
        [HttpGet]
        public async Task<IActionResult> GetTokenAsync([FromBody] LoginRequest body )
        {
            return Ok(body);
        }
    }
}
