﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Areas.Admin.Controllers
{
    public class BankInstallmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}