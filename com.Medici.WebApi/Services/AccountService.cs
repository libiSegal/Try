using com.Medici.WebApi.Auth;
using com.Medici.WebApi.Models;
using com.Medici.WebApi.Objects;
using Newtonsoft.Json;

namespace com.Medici.WebApi.Services
{
    public class AccountService
    {
        readonly JwtUtils jwtUtils;

        public AccountService(JwtUtils jwtUtils)
        {
            this.jwtUtils = jwtUtils;
        }

        //public AuthenticateResponseObject Authenticate(AuthenticateModel model)
        //{
        //    var user = new UserObject()
        //    {
        //        Username = model.Username
        //    };
        //    var token = jwtUtils.GenerateJwtToken(user);

        //    return new AuthenticateResponseObject()
        //    {
        //        Token = token,
        //        Username = "arielcrm",
        //        Name = "Ariel Carmely",
        //    };
        //}
    }
}
