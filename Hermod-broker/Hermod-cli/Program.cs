using System;
using System.Data;
using  Hermod_commons.Beacon;
namespace Hermod_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            UdpBeaconServer udp = new UdpBeaconServer(5000);
            udp.Start();
        }
    }
}