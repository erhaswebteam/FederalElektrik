using Autofac;
using Grand.Core.Configuration;
using Grand.Core.Infrastructure;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, GrandConfig config)
        {
            builder.RegisterType<Grand.Plugin.ContactFormAnswer.Services.ContactUsAnsService>().As<Services.IContactUsAnsService>().InstancePerLifetimeScope();
        }

        public int Order => 1000;
    }
}
