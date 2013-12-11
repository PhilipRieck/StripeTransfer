using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Stripe;

namespace StripeTransfer.StripeSupport
{
    class StripeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KeyManager>()
                .AsSelf()
                .SingleInstance();
            
            builder.Register(c => new StripeAccountService())
                .InstancePerDependency();

            builder.Register(c => new StripeRecipientService())
                .InstancePerDependency();

            builder.Register(c => new StripeBalanceService())
                .InstancePerDependency();

            builder.Register(c => new StripeTransferService())
                .InstancePerDependency();

        }
    }
}
