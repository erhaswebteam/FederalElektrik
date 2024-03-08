using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Web.MobileApp.Models
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}
