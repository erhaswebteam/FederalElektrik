using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Messages;
using Grand.Core.Domain.Points;
using Grand.Data;
using Grand.Framework.Mvc.Filters;
using Grand.Services.Common;
using Grand.Services.Configuration;
using Grand.Services.Customers;
using Grand.Services.Messages;
using Grand.Services.Points;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Web.Areas.Admin.Controllers;
using Grand.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Grand.Plugin.Customer.InsertAndPointProcess.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class InsertAndPointController : BaseAdminController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly DataSettingsManager _dataSettingsManager;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedSMSService _queuedSMSService;
        private readonly IProcessService _processService;
        #endregion

        #region Constructors

        public InsertAndPointController(
            IWorkContext workContext,
            IStoreContext storeContext,
            ISettingService settingService,
            IStoreService storeService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IPermissionService permissionService,
            DataSettingsManager dataSettingsManager,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IMessageTemplateService messageTemplateService,
            IQueuedSMSService queuedSMSService,
            IProcessService processService
            )
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._storeService = storeService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._permissionService = permissionService;
            this._dataSettingsManager = dataSettingsManager;
            this._emailAccountService = emailAccountService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._messageTemplateService = messageTemplateService;
            this._queuedSMSService = queuedSMSService;
            this._processService = processService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
           
            return View("~/Plugins/Customer.InsertAndPointProcess/Views/Customer/Index.cshtml");
        }

        [HttpPost]
        public virtual IActionResult ImportExcel(IFormFile importexcelfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
           
            //try
            //{
            List<Grand.Core.Domain.Customers.Customer> cusList = new List<Grand.Core.Domain.Customers.Customer>();

            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                MongoDBRepository<Grand.Core.Domain.Customers.Customer> mongoDBRepository = new MongoDBRepository<Grand.Core.Domain.Customers.Customer>(_dataSettingsManager.LoadSettings().DataConnectionString);

                using (var package = new ExcelPackage(importexcelfile.OpenReadStream()))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        Grand.Core.Domain.Customers.Customer cus;
                        string _username = workSheet.Cells[rowIterator, 1].Value.ToString().Trim().Replace(" ","");
                        decimal point = workSheet.Cells[rowIterator, 2].Value != null ? decimal.Parse(workSheet.Cells[rowIterator, 2].Value.ToString().Trim()) : 0;
                        string _password = "415263";
                        string firstName = "";
                        string lastName = "";
                        string unvan = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString().Trim() : string.Empty;
                        if (!string.IsNullOrEmpty(unvan))
                        {
                            firstName = unvan.Split(' ').FirstOrDefault();
                            lastName = String.Join(" ", unvan.Split(' ').Skip(1));
                        }
                        string _email = workSheet.Cells[rowIterator, 3].Value.ToString().Trim();
                        //string firstName = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString().Trim() : string.Empty;
                        //string lastname = workSheet.Cells[rowIterator, 7].Value != null ? workSheet.Cells[rowIterator, 7].Value.ToString().Trim() : string.Empty;
                        //string role = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString().Trim() : string.Empty;
                        //string company = workSheet.Cells[rowIterator, 5].Value != null ? workSheet.Cells[rowIterator, 5].Value.ToString().Trim() : string.Empty;
                        //string phone = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString().Trim().Replace(" ", "") : string.Empty;
                        ////string satisYoneticisi = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString().Trim(): string.Empty;
                        //string bolge = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value.ToString().Trim().Replace(" ", "").ToUpper() : string.Empty;
                        //decimal point = workSheet.Cells[rowIterator, 2].Value != null ? decimal.Parse(workSheet.Cells[rowIterator, 2].Value.ToString().Trim()) : 0;

                        cus = mongoDBRepository.Table.Where(x => x.Username == _username).FirstOrDefault();
                        if (cus != null)
                        {
                            //if (point > 0)
                            //{
                            //    Process _proc = new Process()
                            //    {
                            //        Description = DateTime.Now.ToShortDateString() + " puan yükleme.",
                            //        OrderGuid = new Guid(),
                            //        OrderNumber = 0,
                            //        CreatedOnUtc = DateTime.UtcNow,
                            //        Point = point,
                            //        TypeId = (int)ProcessType.Earn,
                            //        Username = _username
                            //    };
                            //    _processService.Insert(_proc);
                            //    var phone= cus.GenericAttributes.Where(x => x.Key == "Phone").FirstOrDefault()?.Value;

                            //    if (string.IsNullOrEmpty(phone) == false)
                            //    {
                            //        var last = "Degerli kullanicimiz,\nHakedis puanlariniz hesaplariniza yuklenmistir.\nIyi alisverisler dileriz.\nEmail:info@promagaza.com\nMusteri destek hatti:0216 213 03 14";
                            //        PassSendSMS(phone, last);
                            //    }
                            //}
                        }
                        else if (cus == null)
                        {
                            cus = new Grand.Core.Domain.Customers.Customer()
                            {
                                CustomerGuid = Guid.NewGuid(),
                                Username = _username,
                                Email = _email,
                                Password = _password,
                                PasswordFormat = PasswordFormat.Clear,
                                CreatedOnUtc = DateTime.Now,
                                Deleted = false,
                                Active = true,
                                IsHasOrders = false
                            };

                            //Role
                            cus.CustomerRoles.Add(_customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered));

                            //cus.CustomerRoles.Add(_customerService.GetCustomerRoleBySystemName("OutsourceFilo"));

                            cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.FirstName, Value = firstName, StoreId = string.Empty });

                            cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.LastName, Value = lastName, StoreId = string.Empty });

                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.Company, Value = "", StoreId = string.Empty });

                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = "SatisYoneticisi", Value = "", StoreId = string.Empty });

                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.Phone, Value = phone, StoreId = string.Empty });

                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = "Bolge", Value = bolge, StoreId = string.Empty });

                            //////Insert Customer
                            mongoDBRepository.Insert(cus);

                            //////Puan Yüklemesi
                            if (point > 0)
                            {
                                Process _proc = new Process()
                                {
                                    Description = DateTime.Now.ToShortDateString() + " puan yükleme.",
                                    OrderGuid = new Guid(),
                                    OrderNumber = 0,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    Point = point,
                                    TypeId = (int)ProcessType.Earn,
                                    Username = _username
                                };
                                _processService.Insert(_proc);
                            }

                            //StringBuilder msg = new StringBuilder();
                            //msg.Append("Degerli Bayimiz;\n");
                            //msg.Append("Viko-port puanınız hesabınıza yüklenmiştir.\n");

                            ////SMS password send
                            //var msg = "Degerli kullanicimiz,\npromagaza.com kullanici bilgileriniz;\nKullanici Adiniz: " + _username + ", Sifreniz: " + _password + "\nIyi alisverisler dileriz.\nEmail:info@promagaza.com\nMusteri destek hatti:0216 213 03 14";
                            //PassSendSMS(phone, msg.ToString());

                            //////Email password send
                            //SendPass(cus.Id);
                        }
                    }
                }

                importexcelfile.OpenReadStream().Dispose();

                ////Mongo DB Kullanıcının eklenmesi
                //if (cusList.Count() > 0)
                //{
                //    mongoDBRepository.Insert(cusList);

                //    foreach (var cus in cusList)
                //    {
                //        SendPass(cus.Id);
                //    }
                //}
            }
            else
            {
                //ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                //return RedirectToAction("List");
            }
            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Imported"));
            //return RedirectToAction("List");
            //}
            //catch (Exception exc)
            //{
            //    ErrorNotification(exc);
            //    return View("~/Plugins/Customer.InsertAndPointProcess/Views/Customer/Index.cshtml");
            //}


            return View("~/Plugins/Customer.InsertAndPointProcess/Views/Customer/Index.cshtml");
        }

        [HttpPost]
        public virtual IActionResult AddMusteriByMusteri(IFormFile importexcelfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try
            //{
            List<Grand.Core.Domain.Customers.Customer> cusList = new List<Grand.Core.Domain.Customers.Customer>();

            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                MongoDBRepository<Grand.Core.Domain.Customers.Customer> mongoDBRepository = new MongoDBRepository<Grand.Core.Domain.Customers.Customer>(_dataSettingsManager.LoadSettings().DataConnectionString);

                using (var package = new ExcelPackage(importexcelfile.OpenReadStream()))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        Grand.Core.Domain.Customers.Customer cus;
                        string _username = workSheet.Cells[rowIterator, 1].Value.ToString().Trim();
                        string _password = "VpKPr24t" + rowIterator.ToString();
                        string _email = _username;
                        string firstName = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString().Trim() : string.Empty;
                        string lastname = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value.ToString().Trim() : string.Empty;
                        string role = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString().Trim() : string.Empty;
                        string company = workSheet.Cells[rowIterator, 5].Value != null ? workSheet.Cells[rowIterator, 5].Value.ToString().Trim() : string.Empty;
                        string phone = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString().Trim().Replace(" ", "") : string.Empty;
                        string vatnumber = workSheet.Cells[rowIterator, 7].Value != null ? workSheet.Cells[rowIterator, 7].Value.ToString().Trim().Replace(" ", "") : string.Empty;
                        string registeredBy = workSheet.Cells[rowIterator, 8].Value != null ? workSheet.Cells[rowIterator, 8].Value.ToString().Trim().Replace(" ", "") : string.Empty;
                        //decimal point = workSheet.Cells[rowIterator, 2].Value != null ? decimal.Parse(workSheet.Cells[rowIterator, 2].Value.ToString().Trim()) : 0;

                        cus = mongoDBRepository.Table.Where(x => x.Username == _username).FirstOrDefault();
                        if (cus == null)
                        {

                            cus = new Grand.Core.Domain.Customers.Customer()
                            {
                                CustomerGuid = Guid.NewGuid(),
                                Username = _username,
                                Email = _email,
                                Password = _password,
                                PasswordFormat = PasswordFormat.Clear,
                                CreatedOnUtc = DateTime.UtcNow,
                                Deleted = false,
                                Active = true
                            };

                            //Role
                            cus.CustomerRoles.Add(_customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered));

                            cus.CustomerRoles.Add(_customerService.GetCustomerRoleBySystemName(role));

                            cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.FirstName, Value = firstName, StoreId = string.Empty });

                            cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.LastName, Value = lastname, StoreId = string.Empty });

                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.Company, Value = company, StoreId = string.Empty });

                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.VatNumber, Value = vatnumber, StoreId = string.Empty });

                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = SystemCustomerAttributeNames.Phone, Value = phone, StoreId = string.Empty });
                            //cus.GenericAttributes.Add(new GenericAttribute() { Key = "RegisteredBy", Value = registeredBy, StoreId = string.Empty });

                            //Insert Customer
                            mongoDBRepository.Insert(cus);

                            ////Puan Yüklemesi
                            //Process _proc = new Process()
                            //{
                            //    Description = DateTime.Now.ToShortDateString() + " puan yükleme.",
                            //    OrderGuid = new Guid(),
                            //    OrderNumber = 0,
                            //    CreatedOnUtc = DateTime.UtcNow,
                            //    Point = point,
                            //    TypeId = (int)ProcessType.Earn,
                            //    Username = _username
                            //};
                            //_processService.Insert(_proc);


                            ////SMS password send
                            //PassSendSMS(phone, msg.ToString());

                            //Email password send
                            //SendPass(cus.Id);
                        }
                       
                    }
                }

                importexcelfile.OpenReadStream().Dispose();
            }
            else
            {
                //ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                //return RedirectToAction("List");
            }
            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Imported"));
            //return RedirectToAction("List");
            //}
            //catch (Exception exc)
            //{
            //    ErrorNotification(exc);
            //    return View("~/Plugins/Customer.InsertAndPointProcess/Views/Customer/Index.cshtml");
            //}


            return View("~/Plugins/Customer.InsertAndPointProcess/Views/Customer/Index.cshtml");
        }

        [HttpPost]
        public virtual IActionResult SendSMSFromExcel(IFormFile importexcelfile)
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                MongoDBRepository<Grand.Core.Domain.Customers.Customer> mongoDBRepository = new MongoDBRepository<Grand.Core.Domain.Customers.Customer>(_dataSettingsManager.LoadSettings().DataConnectionString);

                using (var package = new ExcelPackage(importexcelfile.OpenReadStream()))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        string phone = workSheet.Cells[rowIterator, 1].Value != null ? workSheet.Cells[rowIterator, 1].Value.ToString().Trim().Replace(" ", "") : string.Empty;
                        string msg = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString().Replace("\\n","\n") : string.Empty;
                        
                        PassSendSMS(phone, msg);
                    }
                }
            }
            return View("~/Plugins/Customer.InsertAndPointProcess/Views/Customer/Index.cshtml");
        }

        private bool IsActive(string status)
        {
            bool result = false;

            if (status == "1")
                result = true;

            return result;
        }

        private bool SendEmail(string fromEmailAddress, string fromDisplayName, string fromSubject, string toEmailAddress, string message)
        {
            bool result = false;

            try
            {
                MailMessage ePosta = new MailMessage();
                ePosta.From = new MailAddress(fromEmailAddress, fromDisplayName);
                ePosta.To.Add(toEmailAddress);
                ePosta.Subject = fromSubject;
                ePosta.Body = message;
                ePosta.IsBodyHtml = true;

                int port = 587;
                string host = "smtp-pulse.com";
                string emailaddress = "erkan.domurcuk@erhas.net";
                string password = "nsgZtS7msMGG";

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new System.Net.NetworkCredential(emailaddress, password);
                smtp.Port = port;
                smtp.Host = host;
                smtp.EnableSsl = false;

                try
                {
                    smtp.Send(ePosta);
                    result = true;
                }
                catch (SmtpException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }

        public bool SendPass(string Id)
        {
            var customer = _customerService.GetCustomerById(Id);
            if (customer == null)
                //No customer found with the specified id
                return false;

            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            if (emailAccount == null)
                throw new GrandException("Email account can't be loaded");

            MessageTemplate msg = _messageTemplateService.GetMessageTemplateByName("SendPassword", _storeContext.CurrentStore.Id);

            string body = string.Format(msg.Body, customer.Username, customer.Password);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                EmailAccountId = emailAccount.Id,
                FromName = emailAccount.DisplayName,
                From = emailAccount.Email,
                ToName = customer.GetFullName(),
                To = customer.Email,
                Subject = msg.Subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                DontSendBeforeDateUtc = null
            };
            _queuedEmailService.InsertQueuedEmail(email);
            return true;
        }


        public bool PassSendSMS(string p, string m)
        {
            SendSMS_RequestModel reqSmsModel = new SendSMS_RequestModel();
            reqSmsModel.message = m;
            reqSmsModel.number = p;
            reqSmsModel.token = "FA4048B5-51FB-4FE3-A81D-6511D5FEBDDA";

            PostAPIData postAPIData = new PostAPIData();

            if (postAPIData.SendSMS("http://erhassms.erhas.net/api/smsgateway/SendSMS", reqSmsModel))
            {
                return true;
            }
            else
            {
                return false;
            }
            //QueuedSMS sms = new QueuedSMS();
            //sms.CreatedOnUtc = DateTime.UtcNow;
            //sms.GSM = p.Trim().Replace(" ", "");
            //sms.IsSend = false;
            //sms.Message = m;
            //sms.SendOnUtc = null;

            //_queuedSMSService.InsertQueuedSMS(sms);
           // return true;
        }



        #endregion

    }
}
