using System.Net;

namespace SlotMachine.Infrastructure
{
    public class UserExceptionHandlerMiddleware : AbstractExceptionHandlerMiddleware
    {

        public UserExceptionHandlerMiddleware(RequestDelegate next) : base(next)
        {
        }

        public override (HttpStatusCode code, string message) GetResponse(Exception exception)
        {
            HttpStatusCode code;
            string message = "";
            switch (exception)
            {
                case UserFriendlyException:
                    code = HttpStatusCode.BadRequest;
                    message = ((UserFriendlyException)exception).FriendlyMessage;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    message = "Internal server error";
                    break;
            }
            
            return (code, message);
        }
    }
    public abstract class AbstractExceptionHandlerMiddleware
    {

        private readonly RequestDelegate _next;

        public abstract (HttpStatusCode code, string message) GetResponse(Exception exception);

        public AbstractExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {

                var response = context.Response;
                response.ContentType = "text/plain";

                // get the response code and message
                var (status, message) = GetResponse(exception);
                response.StatusCode = (int)status;
                await response.WriteAsync(message);
            }
        }
    }
}
