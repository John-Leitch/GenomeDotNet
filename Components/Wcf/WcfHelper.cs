using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Components.Wcf
{
    public static class WcfHelper
    {
        private static ServiceHost CreateHost<TService>()
            where TService : new()
        {
            return new ServiceHost(new TService());
        }

        private static string CreateTcpAddress(string host, int port)
        {
            return string.Format("net.tcp://{0}:{1}", host, port);
        }

        private static string CreatePipeAddress(string host, string path)
        {
            return string.Format("net.pipe://{0}/{1}", host, path);
        }

        public static ServiceHost Host<TService, TContract, TBinding>(string address)
            where TBinding : System.ServiceModel.Channels.Binding, new()
            where TService : new()
        {
            var serviceHost = CreateHost<TService>();
            var binding = new TBinding();
            var binding2 = binding as NetTcpBinding;

            if (binding2 != null)
            {
                binding2.Security.Mode = SecurityMode.None;
                binding2.Security.Message.ClientCredentialType = MessageCredentialType.None;            
            }

            serviceHost.AddServiceEndpoint(
                typeof(TContract),
                binding,
                address);

            serviceHost.Open();

            return serviceHost;
        }

        public static ServiceHost HostPipe<TService, TContract>(string host, string path)
            where TService : new()
        {
            var address = CreatePipeAddress(host, path);
            return Host<TService, TContract, NetNamedPipeBinding>(address);
        }

        public static ServiceHost HostPipe<TService, TContract>(string path)
            where TService : new()
        {
            return HostPipe<TService, TContract>(Dns.GetHostName(), path);
        }

        public static ServiceHost HostTcp<TService, TContract>(string host, int port)
            where TService : new()
        {
            var address = CreateTcpAddress(host, port);
            return Host<TService, TContract, NetTcpBinding>(address);            
        }

        public static ServiceHost HostTcp<TService, TContract>(int port)
            where TService : new()
        {
            return HostTcp<TService, TContract>(Dns.GetHostName(), port);
        }

        public static TContract ConnectTcp<TContract>(string host, int port)
        {
            return CreateTcpFactory<TContract>(host, port).CreateChannel();
        }

        public static ChannelFactory<TContract> CreateTcpFactory<TContract>(string host, int port)
        {
            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;

            var clientFactory = new ChannelFactory<TContract>(
                binding,
                CreateTcpAddress(host, port));

            return clientFactory;
        }

        public static TContract ConnectPipe<TContract>(string host, string path)
        {
            var clientFactory = new ChannelFactory<TContract>(
                new NetNamedPipeBinding(),
                CreatePipeAddress(host, path));

            return clientFactory.CreateChannel();
        }
    }
}
