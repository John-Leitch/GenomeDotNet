using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net
{
    public class SmsCarrierTable : Dictionary<SmsCarrier, string>
    {
        public static SmsCarrierTable Default;

        private SmsCarrierTable()
        {

        }

        static SmsCarrierTable()
        {
            Default = new SmsCarrierTable()
            {
                { SmsCarrier.VirginMobileUS, "vmobl.com" },
                { SmsCarrier.StraightTalk, "vtext.com" },
            };
        }
    }
}
