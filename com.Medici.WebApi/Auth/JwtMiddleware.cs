using Microsoft.Extensions.Options;

namespace com.Medici.WebApi.Auth
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, JwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var securityToken = jwtUtils.ValidateJwtToken(token);

                if (securityToken != null)
                {
                    context.Items["Username"] = securityToken.Claims.First(x => string.Equals(x.Type, "Username")).Value;
                }
            }

            await _next(context);
        }
    }
}
