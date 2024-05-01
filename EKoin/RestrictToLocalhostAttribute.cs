using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace EKoin
{
    //Be careful with context.HttpContext.Connection.RemoteIpAddress.
    //If you in forward-proxy mode (some other webserver like IIS or Nginx forwards requests to you) -
    //this ip might always be localhost (because it's actually nginx\iis who makes a request to you), or even null, even for remote requests
    public class RestrictToLocalhostAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            if (!IPAddress.IsLoopback(remoteIp))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
