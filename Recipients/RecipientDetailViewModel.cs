using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Stripe;
using StripeTransfer.Shell;

namespace StripeTransfer
{
    class RecipientDetailViewModel : PropertyChangedBase
    {
        private bool _isDirty;
        private StripeRecipient _original;
        private readonly Lazy<StripeRecipientService> _recipientService;
        private readonly IEventAggregator _eventAggregator;

        public RecipientDetailViewModel(
            StripeRecipient recipient, 
            Lazy<StripeRecipientService> recipientService, 
            IEventAggregator eventAggregator)
        {
            _recipientService = recipientService;
            _eventAggregator = eventAggregator;
            _original = recipient;
            _isNew = true;
            if (recipient != null)
            {
                _isNew = false;
                Name = recipient.Name;
                StripeId = recipient.Id;
                SelectedRecipientType = recipient.Type;
                RoutingNumber = recipient.ActiveAccountBankName;
                AccountNumber = "***";
            }
        }

        public bool IsDirty
        {
            get{ return _isDirty;}
            set
            {
                _isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
            }
        }


        public void Save()
        {
            if (!IsNew)
            {
                MessageBox.Show("Not Implemented");
                return;
            }

            var options = new StripeRecipientCreateOptions();
            options.Name = Name;
            options.Type = SelectedRecipientType;
            options.TaxId = TaxId;
            options.BankAccountCountry = "US";
            options.BankAccountRoutingNumber = RoutingNumber;
            options.BankAccountNumber = AccountNumber;

            _recipientService.Value.Create(options);

        }

        public void Delete()
        {
            var confirmModel = new ConfirmationRequest();
            confirmModel.Title = "really delete recipient?";
            confirmModel.ActionText = "delete " + Name;
            confirmModel.Text = "Deletion can not be undone!";
            confirmModel.Action = () => _recipientService.Value.Delete(StripeId);
            _eventAggregator.PublishOnUIThreadAsync(confirmModel);
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private string _stripeId;
        public string StripeId
        {
            get { return _stripeId; }
            set
            {
                _stripeId = value;
                NotifyOfPropertyChange(() => StripeId);
            }
        }

        private List<string> _types = new List<string> { "individual", "corporation" };
        public List<string> RecipientType
        {
            get { return _types; }
            set
            {
                _types = value;
                NotifyOfPropertyChange(() => RecipientType);
            }
        }

        private string _selectedRecipientType = "individual";
        public string SelectedRecipientType
        {
            get { return _selectedRecipientType; }
            set
            {
                _selectedRecipientType = value;
                NotifyOfPropertyChange(() => SelectedRecipientType);
            }
        }


        private string _taxId;
        public string TaxId
        {
            get { return _taxId; }
            set
            {
                _taxId = value;
                NotifyOfPropertyChange(() => TaxId);
            }
        }

        private string _routingNumber;
        public string RoutingNumber
        {
            get { return _routingNumber; }
            set
            {
                _routingNumber = value;
                NotifyOfPropertyChange(() => RoutingNumber);
            }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get { return _accountNumber; }
            set
            {
                _accountNumber = value;
                NotifyOfPropertyChange(() => AccountNumber);
            }
        }

        private bool _isNew;
        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                NotifyOfPropertyChange(() => IsNew);
            }
        }
    }
}
