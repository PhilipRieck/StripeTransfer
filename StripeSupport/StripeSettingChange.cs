using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeTransfer.StripeSupport
{
    class StripeSettingChange
    {
        public string NewTestKey { get; set; }
        public string NewLiveKey { get; set; }
        public bool? IsLive { get; set; }
    }
}
