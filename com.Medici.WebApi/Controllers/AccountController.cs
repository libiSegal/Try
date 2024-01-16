using com.Medici.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace com.Medici.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public ActionResult Login(AuthenticateModel model)
        {
            try
            {
                return Ok();
                //var response = _accountService.Authenticate(model);

                //return Ok(response);
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;

                if (response is not null && response.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();
            }

            return BadRequest();
        }
    }
}
