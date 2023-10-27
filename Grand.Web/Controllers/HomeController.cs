using Grand.Core;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Customers;
using Grand.Framework.Mvc.Filters;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Points;
using Grand.Web.Models.Catalog;
using Grand.Web.Models.Common;
using Grand.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;

namespace Grand.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        #region Fields
        private readonly IProcessService _processService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IProductWebService _productWebService;
        #endregion

        #region Ctor
        public HomeController(
           IWorkContext workContext,
           ICustomerService customerService,
           IGenericAttributeService genericAttributeService,
           IStoreContext storeContext,
           ISettingService settingService
, IProcessService processService,
           IProductService productService,
           IProductWebService productWebService)
        {
            this._workContext = workContext;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._storeContext = storeContext;
            this._settingService = settingService;
            _processService = processService;
            this._productService = productService;
            this._productWebService = productWebService;
        }
        #endregion


        public virtual IActionResult Index()
        {
            
            return View();
        }

        public virtual IActionResult ChangePass(string RePass, int DateOfBirthDay, int DateOfBirthMonth, int DateOfBirthYear)
        {
            if (!string.IsNullOrEmpty(RePass))
            {
                Customer cus = _workContext.CurrentCustomer;

                cus.PasswordFormat = PasswordFormat.Clear;
                cus.Password = RePass;
                _customerService.UpdateCustomerPassword(cus);
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "IsChangePass", "İlk girişte şifresi değiştirildi.", _storeContext.CurrentStore.Id);

                if (DateOfBirthDay > 0 && DateOfBirthMonth > 0 && DateOfBirthYear > 0)
                {
                    string birthDate = DateOfBirthDay + "-" + DateOfBirthMonth + "-" + DateOfBirthYear;
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "BirthDate", birthDate, "");
                }

                return RedirectToRoute("HomePage");
            }
            else
            {
                return View();
            }
        }

        public virtual IActionResult IsKvkk(IFormCollection form)
        {
            if (form != null)
            {
                //string result = string.Empty;
                //if (!string.IsNullOrEmpty(form["IsShare"]))
                //    result += "IsShare : " + form["IsShare"];
                //if (!string.IsNullOrEmpty(form["IsNews"]))
                //    result += " - IsNews :" + form["IsNews"];

                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "KvkkValues", "OK", _storeContext.CurrentStore.Id);
                return RedirectToRoute("HomePage");
            }
            else
                return View();
        }

        [CheckAccessPublicStore(true)]
        public JsonResult VerifyRecaptcha(string recaptchaToken)
        {
            string apiAddress = "https://www.google.com/recaptcha/api/siteverify";

            string recaptchaSecret = "6Lcj2ggaAAAAABdu0fNPUP2b8rk7O-zf81buvoqW";
            string url = $"{apiAddress}?secret={recaptchaSecret}&response={recaptchaToken}";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    string responseString = httpClient.GetStringAsync(url).GetAwaiter().GetResult();
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseToken>(responseString);
                    return Json(json);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
