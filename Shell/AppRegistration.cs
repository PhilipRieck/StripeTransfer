using System.Linq;
using Autofac;
using Caliburn.Micro;

namespace StripeTransfer
{
    class AppRegistration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .Where(t=> t.IsAssignableTo<IMainTab>())
                .As<IMainTab>()
                .InstancePerDependency();
        }
    }
}
