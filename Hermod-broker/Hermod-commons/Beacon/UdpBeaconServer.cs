using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hermod_commons.Beacon
{
    public class UdpBeaconServer
    {
        public int Port { get; }
        private bool can_run = false;

        public UdpBeaconServer(int port)
        {
            if (port < 1 || port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(port), "Port number must be between 1 and 65535");
            }

            Port = port;
        }

        public void Start()
        {
            can_run = true;
            Run();
        }

        public void Stop()
        {
            can_run = false;
        }


        public void Run()
        {
            // https://stackoverflow.com/questions/4844581/how-do-i-make-a-udp-server-in-c
            byte[] data;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Port);
            UdpClient newsock = new UdpClient(ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            data = newsock.Receive(ref sender);
            Console.WriteLine("Message received from {0}:", sender.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newsock.Send(data, data.Length, sender);

            while(can_run)
            {
                data = newsock.Receive(ref sender);

                Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));
                newsock.Send(data, data.Length, sender);
            }
        }
    }
}