using Autofac;
using Grand.Core.Configuration;
using Grand.Core.Infrastructure;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Plugin.Codec.SendSms.Controllers;

namespace Grand.Plugin.Codec.SendSms
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, GrandConfig config)
        {
            //SCHEDULES
            builder.RegisterType<SMSSendController>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
