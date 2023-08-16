﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class NoDirectAccessAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var canAcess = false;

        // check the refer
        var referer = filterContext.HttpContext.Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            var rUri = new System.UriBuilder(referer).Uri;
            var req = filterContext.HttpContext.Request;
            if (req.Host.Host == rUri.Host && req.Host.Port == rUri.Port && req.Scheme == rUri.Scheme)
            {
                canAcess = true;
            }
        }
        if (!canAcess)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "", action = "", area = "" }));
        }
    }
}