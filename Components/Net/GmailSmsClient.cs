using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net
{
    public class GmailSmsClient : SmtpSmsClient
    {
        public GmailSmsClient(string username, string password)
            : base(new GmailClient(username, password), username)
        {
        }
    }
}
