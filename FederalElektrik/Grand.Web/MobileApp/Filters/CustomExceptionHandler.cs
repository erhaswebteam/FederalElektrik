using Grand.Web.MobileApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Grand.Web.MobileApp.Filters
{
    public class CustomExceptionHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new JsonResult(new ErrorDataResult<Exception>(context.Exception));
        }
    }    
}