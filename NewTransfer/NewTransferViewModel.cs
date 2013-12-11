using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Stripe;
using StripeTransfer.Shell;
using StripeTransfer.StripeSupport;

namespace StripeTransfer
{
    class NewTransferViewModel : Screen, IMainTab, IHandle<StripeSettingChange>
    {
        
        private readonly Lazy<StripeRecipientService> _recipientService;
        private readonly Lazy<StripeBalanceService> _balanceService;
        private readonly Lazy<StripeTransferService> _transferService;
        private readonly IEventAggregator _eventAggregator;
        private readonly KeyManager _keyManager;

        public NewTransferViewModel(
            Lazy<StripeRecipientService> recipientService, 
            Lazy<StripeBalanceService> balanceService, 
            Lazy<StripeTransferService> transferService,
            IEventAggregator eventAggregator, 
            KeyManager keyManager)
        {
            DisplayName = "new transfer";
            _recipientService = recipientService;
            _balanceService = balanceService;
            _transferService = transferService;
            _eventAggregator = eventAggregator;
            _keyManager = keyManager;
            Recipients = new BindableCollection<RecipientViewModel>();
            Recipients.CollectionChanged += OnCollectionChanged;
            Clear();
        }

        protected override void OnActivate()
        {
            if (string.IsNullOrWhiteSpace(Amount))
            {
                Refresh();
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            foreach (var item in Recipients)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
            
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<INotifyPropertyChanged>())
                {
                    item.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckCanTransfer();
        }

        public IObservableCollection<RecipientViewModel> Recipients { get; set; }

        public void Transfer()
        {
            Task.Run(() =>
            {
                _eventAggregator.PublishOnCurrentThread(new ProgressRequest {Message = "initating transfer"});

                try
                {
                    CheckCanTransfer();
                    if (! CanTransfer) return;
                    Issue = "Transfer in progress";
                    var success = 0;

                    foreach (var recipient in Recipients.Where(r=> r.Amount > 0))
                    {
                        var options = new StripeTransferCreateOptions
                        {
                            AmountInCents = recipient.Amount,
                            Currency = "usd",
                            Description = Description,
                            Recipient = recipient.StripeId
                        };

                        try
                        {
                            _transferService.Value.Create(options);
                        }
                        catch (Exception ex)
                        {
                            Issue = success + "done: " + ex.Message;
                            return;
                        }
                        success += 1;
                    }

                    Clear();
                    Issue = "Transfer initiated.";
                }
                catch (Exception e)
                {
                    Clear();
                    Issue = e.Message;
                }
                _eventAggregator.PublishOnCurrentThread(new ProgressRequest { IsComplete = true});
            });
        }

        private void CheckCanTransfer()
        {
            CanTransfer = false;
            if (_realAmount <= 0.10m)
            {
                Issue = "No available funds";
            }
            else if (!Recipients.Any())
            {
                Issue = "No transfer recipients available";
            }
            else if (Recipients.Sum(x => x.Amount) > _realAmount)
            {
                Issue = "Amount to transfer exceeds available.";
            }
            else if (Recipients.Sum(x => x.Amount) <= 10)
            {
                Issue = "Transfer amount too low";
            }
            else
            {
                Issue = "";
                CanTransfer = true;
            }
        }

        private void Clear()
        {
            CanTransfer = false;
            Issue = "Unknown Balance (press refresh)";
            Description = string.Empty;
            Recipients.Clear();
            Amount = string.Empty;
        }


        public int TabOrder { get { return 0; } }

        public new void Refresh()
        {
            if (! _keyManager.IsValid)
            {
                Issue = "Stripe key not set";
            }

            Task.Run(() =>
            {
                _eventAggregator.PublishOnCurrentThread(new ProgressRequest {Message = "retrieving balances"});
                try
                {
                    CanTransfer = false;
                    Recipients.Clear();

                    var balance = _balanceService.Value.Get();
                    _realAmount = balance.Available.First(x => x.Currency == "usd").Amount;
                    Amount = string.Format("{0:C}", (_realAmount/100.0));

                    var candidates = _recipientService.Value.List();
                    Recipients.Add(new RecipientViewModel {Name = "Self", Amount = 0, StripeId = "self"});
                    Recipients.AddRange(candidates.Select(c =>
                        new RecipientViewModel {Name = c.Name, StripeId = c.Id, Amount = 0}));

                    CheckCanTransfer();
                }
                catch (Exception e)
                {
                    Clear();
                    Issue = e.Message;
                }
                _eventAggregator.PublishOnCurrentThread(new ProgressRequest {IsComplete = true});
            });
        }

        private bool _canTransfer;
        public bool CanTransfer
        {
            get { return _canTransfer && string.IsNullOrEmpty(Issue); }
            set
            {
                _canTransfer = value;
                NotifyOfPropertyChange(() => CanTransfer);
            }
        }

        private string _issue;
        public string Issue
        {
            get { return _issue; }
            set
            {
                _issue = value;
                NotifyOfPropertyChange(() => Issue);
            }
        }

        private string _description;
        public string Description
        {
            get{ return _description;}
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        private int _realAmount;
        private string _amount;
        public string Amount
        {
            get{ return _amount;}
            set
            {
                _amount = value;
                NotifyOfPropertyChange(() => Amount);
            }
        }

        public void Handle(StripeSettingChange message)
        {
            if (IsActive)
            {
                Refresh();
            }
            else
            {
                Clear();
            }
        }
    }
}
