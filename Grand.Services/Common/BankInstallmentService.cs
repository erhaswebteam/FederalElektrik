using Grand.Core.Data;
using Grand.Core.Domain.Common;
using System;
using System.Collections.Generic;

namespace Grand.Services.Common
{

    public partial class BankInstallmentService : IBankInstallmentService
    {
        #region Fields
        private readonly IRepository<BankInstallment> _bankInstallmentRepository;

        #endregion

        #region Ctor
        public BankInstallmentService(IRepository<BankInstallment> orderPaymentRepository)
        {
            this._bankInstallmentRepository = orderPaymentRepository;
        }

        #endregion

        public virtual void Insert(List<BankInstallment> bankInstallements)
        {
            if (bankInstallements == null)
                throw new ArgumentNullException("bank_installement");

            foreach (var item in bankInstallements)
            {
                _bankInstallmentRepository.Insert(item);
            }
        }

        public virtual void Update(BankInstallment bankInstallements)
        {
            if (bankInstallements == null)
                throw new ArgumentNullException("bank_installement");

            _bankInstallmentRepository.Update(bankInstallements);
        }
    }
}
