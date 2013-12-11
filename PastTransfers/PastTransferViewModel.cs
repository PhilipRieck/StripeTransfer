using Caliburn.Micro;

namespace StripeTransfer
{
    class PastTransferViewModel : PropertyChangedBase
    {
        private string _date;
        public string Date
        {
            get{ return _date;}
            set
            {
                _date = value;
                NotifyOfPropertyChange(() => Date);
            }
        }

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

        private string _status;
        public string Status
        {
            get{ return _status;}
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        private string _recipient;
        public string Recipient
        {
            get{ return _recipient;}
            set
            {
                _recipient = value;
                NotifyOfPropertyChange(() => Recipient);
            }
        }

    }
}
