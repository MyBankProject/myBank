using MyBankWebApp.Exceptions;

namespace MyBankWebApp.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadReQuestException ex)
            {
                // Ustawiamy tymczasowy komunikat błędu (przez Session + TempData)
                context.Response.Redirect("/User/Login?error=" + Uri.EscapeDataString(ex.Message));
            }
        }
    }
}
