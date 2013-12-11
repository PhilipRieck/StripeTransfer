using System.Configuration;
using Caliburn.Micro;
using Stripe;

namespace StripeTransfer.StripeSupport
{
    class KeyManager : IHandle<StripeSettingChange>
    {
        private string _productionKey;
        private string _testKey;
        private bool _isProduction;

        public bool IsProductionMode
        {
            get { return _isProduction; }
        }

        public void Initialize()
        {
            GetKeys();
            StripeConfiguration.SetApiKey(_testKey);
        }

        private void GetKeys()
        {
            _testKey = ConfigurationManager.AppSettings["testKey"];
            _productionKey = ConfigurationManager.AppSettings["productionKey"];
        }

        private void StoreKeys()
        {
            ConfigurationManager.AppSettings["testKey"] = _testKey;
            ConfigurationManager.AppSettings["productionKey"] = _productionKey;
        }

        public string TestKey { get { return _testKey; } }
        public string LiveKey { get { return _productionKey; } }
        public bool IsValid 
        {
            get
            {
                if (_isProduction) return !string.IsNullOrWhiteSpace(_productionKey);
                return !string.IsNullOrWhiteSpace(_testKey);
            }
        }

        public void Handle(StripeSettingChange message)
        {
            if (message.IsLive.HasValue)
            {
                _isProduction = message.IsLive.Value;
            }
            if (message.NewTestKey != null)
            {
                _testKey = message.NewTestKey;
            }
            if (message.NewLiveKey != null)
            {
                _productionKey = message.NewLiveKey;
            }

            StripeConfiguration.SetApiKey(_isProduction ? _productionKey : _testKey);

            
            if (message.NewLiveKey != null || message.NewTestKey != null)
            {
                StoreKeys();
            }
        }
    }
}
