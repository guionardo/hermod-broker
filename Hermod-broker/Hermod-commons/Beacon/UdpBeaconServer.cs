using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Hermod_commons.Beacon
{
    /// <summary>
    /// UdpBeaconServer
    ///
    /// Listen UDP port, and give response of current IP on LAN
    /// </summary>
    public class UdpBeaconServer
    {
        private readonly List<IPAddress> _myAddresses = new List<IPAddress>();
        private readonly int _port = 0;
        private bool _canRun = false;

        public UdpBeaconServer(int port)
        {
            if (port < 1 || port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(port),
                    "Port number must be between 1 and 65535");
            }

            _port = port;
        }

        public void GetMyAddresses()
        {
            _myAddresses.Clear();
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                //TODO: lançar exceção quando não houver rede disponível
                throw new Exception("Não há rede disponível");
            }

            var ips = new HashSet<IPAddress>();
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var ip in nic.GetIPProperties().DnsAddresses)
                {
                    if (ip.MapToIPv4().GetAddressBytes()[0]==0)
                        continue;
                    ips.Add(ip.MapToIPv4());
                }
            }
            _myAddresses.AddRange(ips.ToList());
        }
        public void Start()
        {
            GetMyAddresses();

            _canRun = true;
            Run();
        }

        private static bool CompareBytes(byte[] b1, byte[] b2, int count)
        {
            if (b1.Length < count || b2.Length < count)
                return false;
            var found = true;
            for (var b = 0; b < count && found; b++)
                found &= b1[b] == b2[b];
            return found;
        }
        
        public IPAddress GetMyIpForClient(IPAddress clientIp)
        {
            var cIpBytes = clientIp.MapToIPv4().GetAddressBytes();
            for (var nBytes = 3; nBytes > 0; nBytes--)
            for (var n = 0; n < nBytes; n++)
                foreach (var myip in _myAddresses.Where(myip => CompareBytes(cIpBytes, myip.GetAddressBytes(), nBytes)))
                    return myip;

            return null;
        }

        public void Stop()
        {
            _canRun = false;
        }


        public void Run()
        {
            // https://stackoverflow.com/questions/4844581/how-do-i-make-a-udp-server-in-c
            byte[] data;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _port);
            UdpClient newsock = new UdpClient(ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            data = newsock.Receive(ref sender);
            Console.WriteLine("Message received from {0}:", sender.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newsock.Send(data, data.Length, sender);
            IPAddress myIP = null;
            while (_canRun)
            {
                data = newsock.Receive(ref sender);
                if (myIP == null)
                    myIP = GetMyIpForClient(sender.Address);

                string response = string.Format("{\"ip\":\"{0}\"}", myIP);
                var responsedata = Encoding.ASCII.GetBytes(response);
                Console.WriteLine(response);
                newsock.Send(responsedata, responsedata.Length, sender);
            }
        }
    }
}