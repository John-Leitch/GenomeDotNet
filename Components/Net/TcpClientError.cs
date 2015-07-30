using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net
{
    public class TcpClientError
    {
        public TcpClient Client { get; private set; }

        public Exception Exception { get; private set; }

        public TcpClientError(TcpClient client, Exception exception)
        {
            Client = client;
            Exception = exception;
        }
    }
}
