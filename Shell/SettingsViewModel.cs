using Caliburn.Micro;
using MahApps.Metro.Controls;
using StripeTransfer.StripeSupport;

namespace StripeTransfer.Shell
{
    class SettingsViewModel : FlyoutBase
    {
        
        private readonly KeyManager _keyManager;
        private readonly IEventAggregator _eventAggregator;

        public SettingsViewModel(KeyManager keyManager, IEventAggregator eventAggregator)
        {
            
            _keyManager = keyManager;
            _eventAggregator = eventAggregator;
            _testKey = _keyManager.TestKey;
            _liveKey = _keyManager.LiveKey;
            IsOpen = false;
            Header = "settings";
            Position = Position.Right;
        }


        private string _testKey;
        public string TestKey
        {
            get{ return _testKey;}
            set
            {
                if (_testKey == value) return;
                _testKey = value;
                NotifyOfPropertyChange(() => TestKey);
                _eventAggregator.PublishOnCurrentThread(new StripeSettingChange {NewTestKey = _testKey});
            }
        }

        private string _liveKey;
        public string LiveKey
        {
            get{ return _liveKey;}
            set
            {
                if (_liveKey == value) return;
                _liveKey = value;
                NotifyOfPropertyChange(() => LiveKey);
                _eventAggregator.PublishOnCurrentThread(new StripeSettingChange { NewLiveKey = _liveKey});
            }
        }

        private bool _isLive;
        public bool IsLive
        {
            get{ return _isLive;}
            set
            {
                if (_isLive == value) return;
                _isLive = value;
                NotifyOfPropertyChange(() => IsLive);
                _eventAggregator.PublishOnCurrentThread(new StripeSettingChange {IsLive = _isLive});
            }
        }
    }
}
