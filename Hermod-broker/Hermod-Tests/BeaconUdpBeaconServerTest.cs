using System.Net;
using NUnit.Framework;
using Hermod_commons.Beacon;

namespace Hermod_Tests
{
    public class BeaconUdpBeaconServerTest
    {

        [Test]
        public void TestBeaconMyIPs()
        {
            var beacon = new UdpBeaconServer(5000);
            beacon.GetMyAddresses();
            var ip = beacon.GetMyIpForClient(new IPAddress(new byte[] {192, 168, 0, 5}));
            Assert.IsNotNull(ip);
        }
    }
}