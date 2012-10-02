using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Battleships
{
    class HostWiFiService : WiFiService
    {
        protected override TcpClient connectSpecific()
        {
            TcpListener host = new TcpListener(System.Net.IPAddress.Any, Global.WIFI_PORT);
            host.Start();

            while (!host.Pending() && isAlive)
                System.Threading.Thread.Sleep(200);

            TcpClient client = null;
            if(isAlive)
                client = host.AcceptTcpClient();
            host.Stop();
            return client;
        }
    }
}
