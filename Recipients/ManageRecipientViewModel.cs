using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Stripe;
using StripeTransfer.Shell;
using StripeTransfer.StripeSupport;

namespace StripeTransfer
{
    class ManageRecipientViewModel : Screen, IMainTab, IHandle<StripeSettingChange>
    {
        private readonly Lazy<StripeRecipientService> _recipientService;
        private readonly Func<StripeRecipient, RecipientDetailViewModel> _recipientDetailFactory;
        private readonly IEventAggregator _eventAggregator;

        public ManageRecipientViewModel(Lazy<StripeRecipientService> recipientService
            , Func<StripeRecipient, RecipientDetailViewModel> recipientDetailFactory, IEventAggregator eventAggregator)
        {
            _recipientService = recipientService;
            _recipientDetailFactory = recipientDetailFactory;
            _eventAggregator = eventAggregator;
            DisplayName = "manage recipients";
            Details = new BindableCollection<RecipientDetailViewModel>();
        }

        public int TabOrder { get { return 2; } }

        protected override void OnActivate()
        {
            if (!Details.Any())
            {
                Refresh();
            }
        }

        public BindableCollection<RecipientDetailViewModel> Details { get; private set; }

        public void Refresh()
        {
            Task.Run(() =>
            {

                _eventAggregator.PublishOnCurrentThread(new ProgressRequest {Message = "retrieving recipients"});
                try
                {
                    Details.Clear();
                    var recipients = _recipientService.Value.List(100);
                    Details.AddRange(recipients
                        .OrderBy(r => r.Name)
                        .Select(r => _recipientDetailFactory(r))
                        );

                    Details.Add(_recipientDetailFactory(null));
                }
                catch (Exception)
                {
                }
                _eventAggregator.PublishOnCurrentThread(new ProgressRequest { IsComplete = true});

            });
        }

        public void Handle(StripeSettingChange message)
        {
            if (IsActive)
            {
                Refresh();
            }
            else
            {
                Details.Clear();
            }
        }
    }
}
