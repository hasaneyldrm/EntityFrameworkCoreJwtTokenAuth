using System;
using System.Security.Claims;
using EntityFrameworkCoreJwtTokenAuth.Interfaces;
using EntityFrameworkCoreJwtTokenAuth.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HashCode = EntityFrameworkCoreJwtTokenAuth.Utilities.HashCode;


namespace EntityFrameworkCoreJwtTokenAuth.Controllers
{
    [Authorize]
    [Route(template: "api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost(template: "authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model: model, ipAddress: IpAddress());

            if (response == null)
                return BadRequest(error: new { message = "Username or password is incorrect" });


            return Ok(value: response);
        }

        [AllowAnonymous]
        [HttpPost(template: "refresh-token")]
        public IActionResult RefreshToken([FromHeader] string refreshToken)
        {
            var response = _userService.RefreshToken(token: refreshToken, ipAddress: IpAddress());

            if (response == null)
                return Unauthorized(value: new { message = "Invalid token" });


            return Ok(value: response);
        }

        [HttpGet]
        [Route(template: "GetIp")]
        public string GetIp()
        {
            var claimsIdentity = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            return claimsIdentity;
        }

        [HttpGet]
        [Route(template: "ModelId")]
        public string GetModelId()
        {
            if (this.User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userId = claimsIdentity.FindFirst(type: ClaimTypes.Name)?.Value;
                string returnId = HashCode.GeneratePublicId(id: Convert.ToInt32(value: userId));
                return returnId;
            }

            return null;
        }

        [HttpGet]
        [Route(template: "GetCompanyId")]
        public string GetCompanyPublicId()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userId = claimsIdentity.FindFirst(type: ClaimTypes.Name)?.Value;
                string publicId = _userService.GetCompany(companyId: Convert.ToInt32(value: userId));
                return publicId;
            }

            return null;
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey(key: "X-Forwarded-For"))
                return Request.Headers[key: "X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
