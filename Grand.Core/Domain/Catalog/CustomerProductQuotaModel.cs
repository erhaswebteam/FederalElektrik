using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a CustomerProductQuotaModel
    /// </summary>
    public partial class CustomerProductQuotaModel
    {
        public CustomerProductQuotaModel()
        {

        }

        public string Username { get; set; }
        public string NameSurname { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string Quota { get; set; }

    }
}
