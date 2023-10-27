using Grand.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Services.Common
{
    /// <summary>
    /// Bank Installement service interface
    /// </summary>
    public partial interface IBankInstallmentService
    {
        void Insert(List<BankInstallment> bankInstallements);

        void Update(BankInstallment bankInstallements);
    }
}
