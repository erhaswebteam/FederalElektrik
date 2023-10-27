using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Grand.Web.CustomFilters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestRateLimitAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public string Page { get; set; }
        public int Seconds { get; set; }
        private static MemoryCache cache { get; } = new MemoryCache(new MemoryCacheOptions());

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            var memoryCacheKey = $"{Name}-{ipAddress}";
            if (!cache.TryGetValue(memoryCacheKey, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));
                cache.Set(memoryCacheKey, true, cacheEntryOptions);
            }
            else
            {
                context.Result = new RedirectResult("./" + Page);
            }
        }
    }
}
