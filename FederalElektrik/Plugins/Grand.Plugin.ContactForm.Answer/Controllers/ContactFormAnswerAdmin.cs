using Grand.Core;
using Grand.Framework.Controllers;
using Grand.Framework.Kendoui;
using Grand.Plugin.ContactFormAnswer.Models;
using Grand.Plugin.ContactFormAnswer.Services;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Messages;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Web.Areas.Admin.Controllers;
using Grand.Web.Areas.Admin.Extensions;
using Grand.Web.Areas.Admin.Models.Customers;
using Grand.Web.Areas.Admin.Models.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer.Controllers
{
    public class SendAnswer
    {
        public string id { get; set; }
        public string answer { get; set; }
    }

    public partial class ContactFormAnswerAdmin : BaseAdminController
    {
        private readonly IContactUsAnsService _contactUsService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ICustomerActivityService _customerActivityService;

        public ContactFormAnswerAdmin(IContactUsAnsService contactUsService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IStoreService storeService,
            IEmailAccountService emailAccountService,
            ICustomerActivityService customerActivityService)
        {
            this._contactUsService = contactUsService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._storeService = storeService;
            this._emailAccountService = emailAccountService;
            this._customerActivityService = customerActivityService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageContactForm))
                return AccessDeniedView();

            var model = new ContactFormListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            return View("~/Plugins/ContactFormAnswer/Views/Admin/List.cshtml", model);
        }

        [HttpPost]
        public IActionResult ContactFormList(DataSourceRequest command, ContactFormListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageContactForm))
                return AccessDeniedView();

            DateTime? startDateValue = (model.SearchStartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.SearchEndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            string vendorId = "";
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var contactform = _contactUsService.GetAllContactUs(
                fromUtc: startDateValue,
                toUtc: endDateValue,
                email: model.SearchEmail,
                storeId: model.StoreId,
                vendorId: vendorId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = contactform.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);

                    var answers = _contactUsService.GetAllContactUsAnswer(x.Id).OrderByDescending(a => a.CreatedOnUtc).ToList();

                    Models.ContactFormModel m = new Models.ContactFormModel();
                    m.Subject = x.Subject;
                    m.Store = store != null ? store.Name : "-empty-";
                    m.IpAddress = x.IpAddress;
                    m.Id = x.Id;
                    m.FullName = x.FullName;
                    m.Enquiry = "";
                    m.EmailAccountName = "";
                    m.Email = x.FullName + " - " + x.Email;
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc); ;
                    m.ContactAttributeDescription = x.ContactAttributeDescription;
                    m.IsAnswered = answers.Count > 0 ? true : false;
                    m.LastAnswerDate = answers.Count > 0 ? _dateTimeHelper.ConvertToUserTime(answers.FirstOrDefault().CreatedOnUtc.Value, DateTimeKind.Utc) : DateTime.MinValue;
                    return m;
                }),
                Total = contactform.TotalCount
            };
            return Json(gridModel);
        }

        public IActionResult Details(string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageContactForm))
                return AccessDeniedView();

            var contactform = _contactUsService.GetContactUsById(id);
            if (contactform == null)
                return RedirectToAction("List");

            var model = contactform.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(contactform.CreatedOnUtc, DateTimeKind.Utc);
            var store = _storeService.GetStoreById(contactform.StoreId);
            model.Store = store != null ? store.Name : "-empty-";
            var email = _emailAccountService.GetEmailAccountById(contactform.EmailAccountId);
            model.EmailAccountName = email != null ? email.DisplayName : "-empty-";
            return View("~/Plugins/ContactFormAnswer/Views/Admin/Details.cshtml", model);
        }

        [HttpPost]
        public IActionResult ContactFormAnswerList(DataSourceRequest command, string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageContactForm))
                return AccessDeniedView();

            var contactform = _contactUsService.GetAllContactUsAnswer(id);

            var gridModel = new DataSourceResult
            {
                Data = contactform.Select(x =>
                {
                    ContactFormAnsModel m = new ContactFormAnsModel();
                    m.Answer = x.Answer;
                    m.CreatedOnUtc = x.CreatedOnUtc;
                    m.FullName = x.FullName;
                    return m;
                }),
                Total = contactform.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult ContactFormAnswerWrite(string id, string answer)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(answer))
                return Json(new { success = false, resultMessage = "Alanlar boş gönderilemez!" });

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageContactForm))
                return AccessDeniedView();

            ContactUsAns contactUsAns = new ContactUsAns();
            contactUsAns.CreatedOnUtc = DateTime.UtcNow;
            contactUsAns.CustomerId = _workContext.CurrentCustomer.Id;
            var currentCustomerFirstName = _workContext.CurrentCustomer.GenericAttributes.Where(x => x.Key == "FirstName").FirstOrDefault();
            var currentCustomerLastName = _workContext.CurrentCustomer.GenericAttributes.Where(x => x.Key == "LastName").FirstOrDefault();
            contactUsAns.FullName = string.Format("{0} {1}", currentCustomerFirstName != null ? currentCustomerFirstName.Value : "", currentCustomerLastName != null ? currentCustomerLastName.Value : "");
            contactUsAns.Answer = answer;
            contactUsAns.ContactFormUsId = id;

            _contactUsService.InsertContactUsAnswer(contactUsAns);

            return Json(new { success = true, resultMessage = "success" });
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageContactForm))
                return AccessDeniedView();

            var contactform = _contactUsService.GetContactUsById(id);
            if (contactform == null)
                //No email found with the specified id
                return RedirectToAction("List");

            _contactUsService.DeleteContactUs(contactform);

            SuccessNotification(_localizationService.GetResource("Admin.System.ContactForm.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("delete-all")]
        public IActionResult DeleteAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageContactForm))
                return AccessDeniedView();

            _contactUsService.ClearTable();

            SuccessNotification(_localizationService.GetResource("Admin.System.ContactForm.DeletedAll"));
            return RedirectToAction("List");
        }
    }
}
