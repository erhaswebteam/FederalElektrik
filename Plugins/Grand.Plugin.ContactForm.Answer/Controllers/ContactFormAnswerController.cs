using Grand.Core;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Messages;
using Grand.Core.Domain.Vendors;
using Grand.Framework.Mvc.Filters;
using Grand.Framework.Security;
using Grand.Framework.Security.Captcha;
using Grand.Plugin.ContactFormAnswer.Models;
using Grand.Plugin.ContactFormAnswer.Services;
using Grand.Services.Customers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Messages;
using Grand.Web.Controllers;
using Grand.Web.Models.Common;
using Grand.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer.Controllers
{
    public class ContactFormAnswerController : BasePublicController
    {
        #region Fields
        private readonly ICommonWebService _commonWebService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerActionEventService _customerActionEventService;
        private readonly IPopupService _popupService;
        private readonly IContactAttributeService _contactAttributeService;
        private readonly CommonSettings _commonSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly VendorSettings _vendorSettings;

        private readonly IContactUsAnsService _contactUsService;

        #endregion

        #region Constructors

        public ContactFormAnswerController(
            ICommonWebService commonWebService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerActivityService customerActivityService,
            ICustomerActionEventService customerActionEventService,
            IPopupService popupService,
            IContactAttributeService contactAttributeService,
            CommonSettings commonSettings,
            CaptchaSettings captchaSettings,
            VendorSettings vendorSettings,
            IContactUsAnsService contactUsService
            )
        {
            this._commonWebService = commonWebService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerActivityService = customerActivityService;
            this._customerActionEventService = customerActionEventService;
            this._popupService = popupService;
            this._contactAttributeService = contactAttributeService;
            this._commonSettings = commonSettings;
            this._captchaSettings = captchaSettings;
            this._vendorSettings = vendorSettings;
            this._contactUsService = contactUsService;
        }

        #endregion

        #region Methods

        //contact us page
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult ContactUs(string id)
        {
            ContactUsAnsModel model = new ContactUsAnsModel();

            var _model = _commonWebService.PrepareContactUs();

            if (String.IsNullOrWhiteSpace(id))
            {
                model = new ContactUsAnsModel
                {
                    ContactAttributeInfo = _model.ContactAttributeInfo,
                    ContactAttributes = _model.ContactAttributes,
                    ContactAttributeXml = _model.ContactAttributeXml,
                    CustomProperties = _model.CustomProperties,
                    DisplayCaptcha = _model.DisplayCaptcha,
                    Email = _model.Email,
                    Enquiry = _model.Enquiry,
                    FullName = _model.FullName,
                    Result = _model.Result,
                    Subject = _model.Subject,
                    SubjectEnabled = _model.SubjectEnabled,
                    SuccessfullySent = _model.SuccessfullySent
                };

                var contactusList = _contactUsService.GetAllContactUs(customerId: _workContext.CurrentCustomer.Id);
                model.ContactUsList = contactusList.ToList();
                foreach (var item in model.ContactUsList)
                {
                    var answer = _contactUsService.GetAllContactUsAnswer(item.Id);
                    if (answer != null && answer.Count > 0)
                        model.IsAnswered.Add(true);
                    else
                        model.IsAnswered.Add(false);
                }

                return View("~/Plugins/ContactFormAnswer/Views/ContactUs.cshtml", model);
            }
            else
            {
                model = new ContactUsAnsModel();

                var contactus = _contactUsService.GetAllContactUs(customerId: _workContext.CurrentCustomer.Id).Where(x => x.Id == id).FirstOrDefault();
                if (contactus != null)
                {
                    model.Subject = contactus.Subject;
                    model.Enquiry = contactus.Enquiry;
                    model.ContactUsAnsList = _contactUsService.GetAllContactUsAnswer(contactus.Id).ToList();
                    model.CreateDate = contactus.CreatedOnUtc;
                }

                var contactusList = _contactUsService.GetAllContactUs(customerId: _workContext.CurrentCustomer.Id);
                model.ContactUsList = contactusList.ToList();
                foreach (var item in model.ContactUsList)
                {
                    var answer = _contactUsService.GetAllContactUsAnswer(item.Id);
                    if (answer != null && answer.Count > 0)
                        model.IsAnswered.Add(true);
                    else
                        model.IsAnswered.Add(false);
                }

                return View("~/Plugins/ContactFormAnswer/Views/ContactUsDetail.cshtml", model);
            }
        }

        [HttpPost, ActionName("ContactUs")]
        [PublicAntiForgery]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult ContactUsSend(ContactUsAnsModel model, IFormCollection form, bool captchaValid,
            [FromServices] IContactAttributeFormatter contactAttributeFormatter)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            //parse contact attributes
            var attributeXml = _commonWebService.ParseContactAttributes(form);
            var contactAttributeWarnings = _commonWebService.GetContactAttributesWarnings(attributeXml);
            if (contactAttributeWarnings.Any())
            {
                foreach (var item in contactAttributeWarnings)
                {
                    ModelState.AddModelError("", item);
                }
            }

            if (ModelState.IsValid)
            {
                model.ContactAttributeXml = attributeXml;
                model.ContactAttributeInfo = contactAttributeFormatter.FormatAttributes(attributeXml, _workContext.CurrentCustomer);
                var _model = _commonWebService.SendContactUs(model);

                model.ContactAttributeInfo = _model.ContactAttributeInfo;
                model.ContactAttributes = _model.ContactAttributes;
                model.ContactAttributeXml = _model.ContactAttributeXml;
                model.CustomProperties = _model.CustomProperties;
                model.DisplayCaptcha = _model.DisplayCaptcha;
                model.Email = _model.Email;
                model.Enquiry = _model.Enquiry;
                model.FullName = _model.FullName;
                model.Result = _model.Result;
                model.Subject = _model.Subject;
                model.SubjectEnabled = _model.SubjectEnabled;
                model.SuccessfullySent = _model.SuccessfullySent;

                var contactusList = _contactUsService.GetAllContactUs(customerId: _workContext.CurrentCustomer.Id);
                model.ContactUsList = contactusList.ToList();
                foreach (var item in model.ContactUsList)
                {
                    var answer = _contactUsService.GetAllContactUsAnswer(item.Id);
                    if (answer != null && answer.Count > 0)
                        model.IsAnswered.Add(true);
                    else
                        model.IsAnswered.Add(false);
                }

                //activity log
                _customerActivityService.InsertActivity("PublicStore.ContactUs", "", _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));
                return View("~/Plugins/ContactFormAnswer/Views/ContactUs.cshtml", model);
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            model.ContactAttributes = _commonWebService.PrepareContactAttributeModel(attributeXml);

            return View("~/Plugins/ContactFormAnswer/Views/ContactUs.cshtml", model);
        }

        #endregion
    }
}