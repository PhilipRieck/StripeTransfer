using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using StripeTransfer.Shell;

namespace StripeTransfer
{
    class Bootstrap : BootstrapperBase
    {
        public IContainer _container;

        public Bootstrap()
        {
            Start();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }

        protected override void BuildUp(object instance)
        {
            _container.InjectProperties(instance);
        }
        
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.Resolve(typeof (IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance = null;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (_container.TryResolve(service, out instance))
                    return instance;
            }
            else
            {
                if (_container.TryResolveKeyed(key, service, out instance))
                    return instance;
            }
            throw new ArgumentException("No instances of specified contract " + service.Name, "service");
        }

        protected override void Configure()
        {
            base.Configure();

            var builder = new ContainerBuilder();

            builder.RegisterType<WindowManager>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<EventAggregator>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .Where(t => t.Name.EndsWith("ViewModel") || t.Name.EndsWith("View"))
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterAssemblyModules(AssemblySource.Instance.ToArray());

            builder.RegisterModule<EventAggregatorSubscriptionModule>();

            _container = builder.Build();

            ConfigureConventions();
        }

        private void ConfigureConventions()
        {
            ConventionManager.AddElementConvention<UIElement>(UIElement.VisibilityProperty, "Visibility", "VisibilityChanged");

            var baseBindProperties = ViewModelBinder.BindProperties;
            ViewModelBinder.BindProperties =
                (frameWorkElements, viewModel) =>
                {
                    BindVisiblityProperties(frameWorkElements, viewModel);
                    return baseBindProperties(frameWorkElements, viewModel);
                };

            // Need to override BindActions as well, as it's called first and filters out anything it binds to before
            // BindProperties is called.
            var baseBindActions = ViewModelBinder.BindActions;
            ViewModelBinder.BindActions =
                (frameWorkElements, viewModel) =>
                {
                    BindVisiblityProperties(frameWorkElements, viewModel);
                    return baseBindActions(frameWorkElements, viewModel);
                };

            //Bind other collections (like flyouts)
            var getNamedElements = BindingScope.GetNamedElements;
            BindingScope.GetNamedElements = o =>
            {
                var list = new List<FrameworkElement>(getNamedElements(o));

                var metroWindow = o as MetroWindow;
                if (metroWindow != null)
                {
                    var type = o.GetType();
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Where(f => f.DeclaringType == type);
                    var flyouts = fields.Where(f => f.FieldType == typeof (FlyoutsControl))
                        .Select(f => f.GetValue(o))
                        .Cast<FlyoutsControl>();
                    list.AddRange(flyouts);
                }
                return list;
            };

        }

        void BindVisiblityProperties(IEnumerable<FrameworkElement> frameWorkElements, Type viewModel)
        {
            foreach (var frameworkElement in frameWorkElements)
            {
                var propertyName = frameworkElement.Name + "IsVisible";
                var property = viewModel.GetPropertyCaseInsensitive(propertyName);
                if (property != null)
                {
                    var convention = ConventionManager
                        .GetElementConvention(typeof(FrameworkElement));
                    ConventionManager.SetBindingWithoutBindingOverwrite(
                        viewModel,
                        propertyName,
                        property,
                        frameworkElement,
                        convention,
                        convention.GetBindableProperty(frameworkElement));
                }
            }
        }
    }
}
