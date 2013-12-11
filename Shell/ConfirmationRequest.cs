using Action = System.Action;

namespace StripeTransfer.Shell
{
    class ConfirmationRequest
    {
        
        public bool IsConfirmed;
        public string Title = "Are you sure?";
        public string Text;
        public string ActionText = "Confirm";
        public Action Action;
    }
}
