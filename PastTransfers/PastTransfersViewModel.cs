using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Stripe;
using StripeTransfer.Shell;
using StripeTransfer.StripeSupport;

namespace StripeTransfer
{
    class PastTransfersViewModel : Screen, IMainTab, IHandle<StripeSettingChange>
    {
        private readonly Lazy<StripeTransferService> _transferService;
        private readonly Lazy<StripeRecipientService> _recipientService;
        private Dictionary<string, string> _recipientMap;
        private readonly IEventAggregator _eventAggregator;

        public PastTransfersViewModel(Lazy<StripeTransferService> transferService, Lazy<StripeRecipientService> recipientService, IEventAggregator eventAggregator)
        {
            _transferService = transferService;
            _recipientService = recipientService;
            _eventAggregator = eventAggregator;
            DisplayName = "past transfers";
            Transfers = new BindableCollection<PastTransferViewModel>();
        }

        protected override void OnActivate()
        {
            if (!Transfers.Any())
            {
                Refresh();
            }
        }

        public int TabOrder { get { return 1; } }

        public void Refresh()
        {
            Task.Run(() =>
            {
                _eventAggregator.PublishOnCurrentThread(new ProgressRequest { Message = "retrieving balances" });
                try
                {
                    
                Transfers.Clear();

                var transfers = _transferService.Value.List(20);
                Transfers.AddRange(
                    transfers
                        .OrderByDescending(t => t.Date)
                        .Select(t => new PastTransferViewModel
                        {
                            Amount = string.Format("{0:C}", t.AmountInCents/100.0),
                            Date = t.Date.ToShortDateString(),
                            Status = t.Status,
                            Recipient = GetRecipientName(t.Recipient),
                        }
                        ));
                }
                catch (Exception)
                {
                }
                _eventAggregator.PublishOnCurrentThread(new ProgressRequest { IsComplete = true});
            });
        }

        private string GetRecipientName(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return "Self";

            if (_recipientMap == null)
            {
                _recipientMap = new Dictionary<string, string>();
                var recipients = _recipientService.Value.List(20);
                foreach (var item in recipients)
                {
                    _recipientMap.Add(item.Id, item.Name);
                }
            }
            if (!_recipientMap.ContainsKey(id))
            {
                var recipient = _recipientService.Value.Get(id);
                if (recipient != null)
                {
                    _recipientMap.Add(recipient.Id, recipient.Name);
                }
            }
            if (_recipientMap.ContainsKey(id))
            {
                return _recipientMap[id];
            }
            return "Unknown";
        }

        public BindableCollection<PastTransferViewModel> Transfers { get; private set; }

        public void Handle(StripeSettingChange message)
        {
            if (IsActive)
            {
                Refresh();
            }
            else
            {
                Transfers.Clear();
            }

        }
    }
}
