using Autofac;
using Grand.Core.Configuration;
using Grand.Core.Data;
using Grand.Core.Infrastructure;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Plugin.Customer.InsertAndPointProcess.Controllers;

namespace Grand.Plugin.Customer.InsertAndPointProcess
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, GrandConfig config)
        {
            builder.RegisterType<DataSettingsManager>();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
