﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Web.MobileApp.Models
{
    public interface IDataResult<out T>:IResult
    {
        T Data { get; }
    }
}