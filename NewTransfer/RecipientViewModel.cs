using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace StripeTransfer
{
    class RecipientViewModel : PropertyChangedBase
    {


        private string _name;
        public string Name
        {
            get{ return _name;}
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
        
        private string _stripeId;
        public string StripeId
        {
            get{ return _stripeId;}
            set
            {
                _stripeId = value;
                NotifyOfPropertyChange(() => StripeId);
            }
        }
        private int _amount;
        public int Amount
        {
            get{ return _amount;}
            set
            {
                _amount = value;
                NotifyOfPropertyChange(() => Amount);
            }
        }
        public void Remove() { }
    }
}
