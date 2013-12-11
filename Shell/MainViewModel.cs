using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StripeTransfer.Shell;
using StripeTransfer.StripeSupport;

namespace StripeTransfer
{
    class MainViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<ConfirmationRequest>, IHandle<ProgressRequest>, IHandle<StripeSettingChange>
    {
        private readonly Func<SettingsViewModel> _settingsFactory;
        private const string _baseDisplay = "Stripe Transferinator";

        public MainViewModel(
            IEnumerable<IMainTab> mainTabViewModels, 
            Func<SettingsViewModel> settingsFactory, 
            KeyManager keyManager) : base()
        {
            _settingsFactory = settingsFactory;
            keyManager.Initialize();
            DisplayName = _baseDisplay + (keyManager.IsProductionMode ? " ( LIVE )" : "( test )");
            Items.AddRange(mainTabViewModels.OrderBy(x=> x.TabOrder));
            _flyouts = new BindableCollection<FlyoutBase>();
        }

        public void Quit()
        {
            TryClose();
        }

        protected override void OnInitialize()
        {
            _flyouts.Add(_settingsFactory());
        }

        private BindableCollection<FlyoutBase> _flyouts;
        public BindableCollection<FlyoutBase> Flyouts 
        {
            get{ return _flyouts;}
            set
            {
                _flyouts = value;
                NotifyOfPropertyChange(() => Flyouts);
            }
        }

        public void OpenSettings()
        {
            var flyout = _flyouts.First();
            flyout.IsOpen = !flyout.IsOpen;
        }

        public bool CanOpenSettings()
        {
            return true;
        }


        public void Handle(ConfirmationRequest message)
        {
            var view = GetView() as MetroWindow;
            if (view == null) return;

            view.MetroDialogOptions.AffirmativeButtonText = message.ActionText;
            view.ShowMessageAsync(message.Title, message.Text, MessageDialogStyle.AffirmativeAndNegative)
                .ContinueWith(r => { if (r.Result == MessageDialogResult.Affirmative) message.Action(); });
            
        }


        private Task<ProgressDialogController> _progressTask;
        public void Handle(ProgressRequest message)
        {
            if (message.IsComplete)
            {
                _progressTask.Result.CloseAsync();
                _progressTask = null;
                return;
            }

            var view = GetView() as MetroWindow;
            if (view == null) return;
            if (view.Dispatcher.CheckAccess())
            {
                InternalHandleProgress(view,message);
            }
            else
            {
                view.Dispatcher.Invoke(() => InternalHandleProgress(view, message));
            }
        }

        private void InternalHandleProgress(MetroWindow view, ProgressRequest message)
        {
            _progressTask = view.ShowProgressAsync("Please wait...", message.Message);
        }


        private Theme _currentTheme = Theme.Light;
        public void Handle(StripeSettingChange message)
        {
            if (message.IsLive.HasValue)
            {
                var view = GetView() as Window;

                var accentName = message.IsLive.Value ? "Red" : "Emerald";
                var accent = ThemeManager.DefaultAccents.First(a => a.Name == accentName);
                ThemeManager.ChangeTheme(view, accent, _currentTheme);
                DisplayName = _baseDisplay + (message.IsLive.Value ? " ( LIVE )" : "( test )");
            }
        }
    }
}
