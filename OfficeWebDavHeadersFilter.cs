using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OfficeWebDav
{
    public class OfficeWebDavHeadersFilter: ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers.Add("dav", "1,2");
            context.HttpContext.Response.Headers.Add("www-authenticate", "Anonymous");
            context.HttpContext.Response.Headers.Add("access-control-allow-origin", "*");
            context.HttpContext.Response.Headers.Add("access-control-allow-credentials", "true");
            context.HttpContext.Response.Headers.Add("ms-author-via", "DAV");
            context.HttpContext.Response.Headers.Add("access-control-expose-headers", "DAV, content-length, Allow");
            //context.HttpContext.Response.Headers.Add("allow", "PROPPATCH,PROPFIND,OPTIONS,DELETE,UNLOCK,COPY,LOCK,MOVE,HEAD,POST,PUT,GET");
            context.HttpContext.Response.Headers.Add("allow", "OPTIONS,UNLOCK,LOCK,HEAD,PUT,GET");
            base.OnActionExecuted(context);
        }
    }
}
