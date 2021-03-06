﻿using Autofac;
using Autofac.Core;
using Caliburn.Micro;

namespace StripeTransfer.Shell
{
    internal class EventAggregatorSubscriptionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry registry,
            IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        private static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            //  we never want to fail, so check for null (should never happen), and return if it is
            if (e == null)
                return;
            //  try to convert instance to IHandle
            //  I originally did e.Instance.GetType().IsAssignableTo<>() and then 'as', 
            //  but it seemed redundant
            var handler = e.Instance as IHandle;
            //  if it is not null, it implements, so subscribe
            if (handler != null)
                e.Context.Resolve<IEventAggregator>().Subscribe(handler);
        }
    }
}
