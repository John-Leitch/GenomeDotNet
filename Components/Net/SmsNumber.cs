using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net
{
    public class SmsNumber
    {
        public SmsCarrier Carrier { get; set; }

        public string Number { get; set; }

        public SmsNumber()
        {
        }

        public SmsNumber(SmsCarrier carrier, string number)
        {
            Carrier = carrier;
            Number = number;
        }
    }
}
