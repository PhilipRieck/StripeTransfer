using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace StripeTransfer.Shell
{
    class FlyoutBase : Screen
    {
        private string _header;
        public string Header
        {
            get{ return _header;}
            set
            {
                _header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get{ return _isOpen;}
            set
            {
                _isOpen = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        private Position _position = Position.Right;
        public Position Position
        {
            get{ return _position;}
            set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);
            }
        }
    }
}
