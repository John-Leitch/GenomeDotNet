using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class RemoteConsole : IDisposable
    {
        private ConsoleServerComponent _component;

        private ClientContext _context;

        public Guid Id { get; private set; }

        public RemoteConsole(ConsoleServerComponent server, ClientContext context, Guid id)
        {
            _component = server;
            _context = context;
            Id = id;
        }

        public string ExecuteCommand(string command)
        {
            var id = "CMD:" + Guid.NewGuid().ToString();
            _component.Network.WriteObject(_context, new ConsoleMessage(ConsoleMessageType.Write, Id, command + "\r\nECHO " + id + "\r\n"));
            var resp = _component.ExpectConsoleMessage(_context, ConsoleMessageType.Write, Id);

            var respStr = "";
            var buffer = "";
            do
            {
                buffer = ReadResponse();
                respStr += buffer;
            } while (!Regex.IsMatch(respStr.ToString(), id + @".*>", RegexOptions.Singleline));

            var lines = respStr.SplitLines();
            respStr = lines.Take(lines.Length - 4).Join("\r\n") + "\r\n" + lines.Last();
            return respStr;
        }

        public string ReadResponse()
        {
            var respStr = new StringBuilder();
            ConsoleMessage resp;

            do
            {
                _component.Network.WriteObject(_context, new ConsoleMessage(ConsoleMessageType.Read, Id));

            //retry:
                //try
                //{
                    
                    resp = _component.ExpectConsoleMessage(_context, ConsoleMessageType.Read, Id);
                //}
                //catch (InvalidOperationException)
                //{
                //    goto retry;
                //}

                if (resp.Data != null)
                {
                    respStr.Append(resp.Data);
                    Thread.Sleep(200);
                }
            } while (resp.Data != null);

            return respStr.ToString();
        }

        public void Dispose()
        {
            _component.WriteConsoleMessage(_context, ConsoleMessageType.Unregister, Id);
            var resp = _component.ExpectConsoleMessage(_context, ConsoleMessageType.Unregister, Id);
        }
    }
}
