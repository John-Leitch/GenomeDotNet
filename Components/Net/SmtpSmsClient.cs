using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net
{
    public class SmtpSmsClient : IDisposable
    {
        public SmtpClient SmtpClient { get; private set; }

        public string From { get; private set; }

        public SmtpSmsClient(SmtpClient smtpClient, string from)
        {
            SmtpClient = smtpClient;
            From = from;
        }

        public void Send(SmsCarrier carrier, string number, string message)
        {
            SmtpClient.Send(
                From,
                string.Format("{0}@{1}", number, SmsCarrierTable.Default[carrier]),
                null,
                message);
        }

        public void Send(SmsNumber number, string message)
        {
            Send(number.Carrier, number.Number, message);
        }

        public void Dispose()
        {
            if (SmtpClient != null)
            {
                SmtpClient.Dispose();
            }
        }
    }
}
