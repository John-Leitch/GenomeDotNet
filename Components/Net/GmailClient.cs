using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net
{
    public class GmailClient : SmtpClient
    {
        public GmailClient(string username, string password)
            : base("smtp.gmail.com", 25)
        {
            Credentials = new NetworkCredential(username, password);
            EnableSsl = true;
        }
    }
}
