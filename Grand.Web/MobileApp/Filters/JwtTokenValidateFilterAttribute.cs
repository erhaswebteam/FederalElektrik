using Grand.Core;
using Grand.Core.Domain.Customers;
using Grand.Services.Authentication;
using Grand.Services.Common;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Events;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Orders;
using Grand.Web.MobileApp.Models;
using Grand.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Grand.Web.MobileApp.Filters
{
    public class JwtTokenValidateFilterAttribute : Attribute, IActionFilter
    {
        private IHostingEnvironment _hostingEnvironment;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var isDiabled = (context.ActionDescriptor as ControllerActionDescriptor)
                .MethodInfo.GetCustomAttributes(typeof(IngoreAllAttributes), true)
                .Length > 0;

            if (isDiabled)
                return;

            _hostingEnvironment = (IHostingEnvironment)context.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            
            var config = new ConfigurationBuilder()
                .SetBasePath(_hostingEnvironment.ContentRootPath)
                .AddJsonFile("App_Data/appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var key = config["Jwt:key"];
            var token = context.HttpContext.Request.Headers["token"];

            if(string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new ErrorResult("Token data not found"));
                return;
            }

            var result = JwtParserService.ValidateJwtToken(token.ToString(), key);

            if(!result)
                context.Result = new JsonResult(new ErrorResult("Token is not valid"));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }

    public class IngoreAllAttributes : Attribute
    {

    }
}
