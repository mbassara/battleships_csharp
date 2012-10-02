using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Battleships
{
    class ClientWiFiService : WiFiService
    {
        private String hostIP;
        public String HostIP
        {
            get { return hostIP; }
            set { hostIP = value; }
        }

        public ClientWiFiService()
        {
        }

        public ClientWiFiService(String ip)
        {
            this.hostIP = ip;
        }

        protected override TcpClient connectSpecific()
        {
            TcpClient client = null;
            client = new TcpClient();
            IAsyncResult result = client.BeginConnect(System.Net.IPAddress.Parse(hostIP), Global.WIFI_PORT, null, null);
            do
            {
                System.Diagnostics.Debug.WriteLine("TCPClient connecting with: " + hostIP);
                System.Threading.Thread.Sleep(200);
            } while (!result.IsCompleted && isAlive);

            if (!isAlive)
                client = null;
            else
            {
                try { client.EndConnect(result); }
                catch (SocketException) {
                    client = null;
                    connectionTimeout = true;
                }
            }

            return client;
        }
    }
}
