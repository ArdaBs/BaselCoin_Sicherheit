using NoSQLSkiServiceManager.Services;

namespace NoSQLSkiServiceManager.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, TokenService tokenService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null && tokenService.IsTokenExpired(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token is expired");
                return;
            }

            await _next(context);
        }
    }

}
