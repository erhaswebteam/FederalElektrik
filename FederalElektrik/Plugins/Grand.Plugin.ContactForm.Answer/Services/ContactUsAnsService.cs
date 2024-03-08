using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Domain.Messages;
using Grand.Plugin.ContactFormAnswer.Models;
using Grand.Services.Events;
using Grand.Services.Messages;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer.Services
{
    public partial class ContactUsAnsService : ContactUsService, IContactUsAnsService
    {
        private readonly IRepository<ContactUs> _contactusRepository;
        private readonly IRepository<ContactUsAns> _answerRepository;
        private readonly IEventPublisher _eventPublisher;

        public ContactUsAnsService(
            IRepository<ContactUs> contactusRepository,
            IEventPublisher eventPublisher,
            IRepository<ContactUsAns> answerRepository) : base(contactusRepository, eventPublisher)
        {
            this._contactusRepository = contactusRepository;
            this._eventPublisher = eventPublisher;
            this._answerRepository = answerRepository;
        }

        /// <summary>
        /// Deletes a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs item</param>
        public override void DeleteContactUs(ContactUs contactus)
        {
            if (contactus == null)
                throw new ArgumentNullException("contactus");

            _contactusRepository.Delete(contactus);

            //event notification
            _eventPublisher.EntityDeleted(contactus);

        }

        /// <summary>
        /// Clears table
        /// </summary>
        public override void ClearTable()
        {
            _contactusRepository.Collection.DeleteMany(new MongoDB.Bson.BsonDocument());
        }

        /// <summary>
        /// Gets all contactUs items
        /// </summary>
        /// <param name="fromUtc">ContactUs item creation from; null to load all records</param>
        /// <param name="toUtc">ContactUs item creation to; null to load all records</param>
        /// <param name="email">email</param>
        /// <param name="vendorId">vendorId; null to load all records</param>
        /// <param name="customerId">customerId; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>ContactUs items</returns>
        public override IPagedList<ContactUs> GetAllContactUs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string email = "", string vendorId = "", string customerId = "", string storeId = "",
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var builder = Builders<ContactUs>.Filter;
            var filter = builder.Where(c => true);

            if (fromUtc.HasValue)
                filter = filter & builder.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                filter = filter & builder.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (!String.IsNullOrEmpty(vendorId))
                filter = filter & builder.Where(l => vendorId == l.VendorId);
            if (!String.IsNullOrEmpty(customerId))
                filter = filter & builder.Where(l => customerId == l.CustomerId);
            if (!String.IsNullOrEmpty(storeId))
                filter = filter & builder.Where(l => storeId == l.StoreId);

            if (!String.IsNullOrEmpty(email))
                filter = filter & builder.Where(l => l.Email.ToLower().Contains(email.ToLower()));

            var builderSort = Builders<ContactUs>.Sort.Descending(x => x.CreatedOnUtc);
            var query = _contactusRepository.Collection;
            var contactus = new PagedList<ContactUs>(query, filter, builderSort, pageIndex, pageSize);

            return contactus;
        }

        /// <summary>
        /// Gets a contactus item
        /// </summary>
        /// <param name="contactUsId">ContactUs item identifier</param>
        /// <returns>ContactUs item</returns>
        public override ContactUs GetContactUsById(string contactUsId)
        {
            return _contactusRepository.GetById(contactUsId);
        }

        /// <summary>
        /// Inserts a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs</param>
        /// <returns>A contactus item</returns>
        public override void InsertContactUs(ContactUs contactus)
        {
            if (contactus == null)
                throw new ArgumentNullException("contactus");

            _contactusRepository.Insert(contactus);

            //event notification
            _eventPublisher.EntityInserted(contactus);

        }

        #region Answers



        /// <summary>
        /// Gets all contactUs items
        /// </summary>
        /// <param name="fromUtc">ContactUs item creation from; null to load all records</param>
        /// <param name="toUtc">ContactUs item creation to; null to load all records</param>
        /// <param name="email">email</param>
        /// <param name="vendorId">vendorId; null to load all records</param>
        /// <param name="customerId">customerId; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>ContactUs items</returns>
        public virtual IPagedList<ContactUsAns> GetAllContactUsAnswer(string contactUsId)
        {
            var builder = Builders<ContactUsAns>.Filter;
            var filter = builder.Where(c => true);

            filter = filter & builder.Where(l => l.ContactFormUsId == contactUsId);

            //if (fromUtc.HasValue)
            //    filter = filter & builder.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            //if (toUtc.HasValue)
            //    filter = filter & builder.Where(l => toUtc.Value >= l.CreatedOnUtc);
            //if (!String.IsNullOrEmpty(vendorId))
            //    filter = filter & builder.Where(l => vendorId == l.VendorId);
            //if (!String.IsNullOrEmpty(customerId))
            //    filter = filter & builder.Where(l => customerId == l.CustomerId);
            //if (!String.IsNullOrEmpty(storeId))
            //    filter = filter & builder.Where(l => storeId == l.StoreId);

            //if (!String.IsNullOrEmpty(email))
            //    filter = filter & builder.Where(l => l.Email.ToLower().Contains(email.ToLower()));

            var builderSort = Builders<ContactUsAns>.Sort.Descending(x => x.CreatedOnUtc);
            var query = _answerRepository.Collection;
            var contactus = new PagedList<ContactUsAns>(query, filter, builderSort, 0, 1000);

            return contactus;
        }

        /// <summary>
        /// Inserts a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs</param>
        /// <returns>A contactus item</returns>
        public virtual void InsertContactUsAnswer(ContactUsAns contactusans)
        {
            if (contactusans == null)
                throw new ArgumentNullException("contactus");

            _answerRepository.Insert(contactusans);

            //event notification
            _eventPublisher.EntityInserted(contactusans);

        }

        #endregion
    }
}
