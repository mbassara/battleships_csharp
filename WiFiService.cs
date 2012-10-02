using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Threading;

namespace Battleships
{
    abstract class WiFiService
    {
        private TcpClient client = null;
        private NetworkStream stream = null;
        private System.IO.BinaryReader binaryReader = null;
        private System.IO.BinaryWriter binaryWriter = null;
        private System.IO.StreamReader reader = null;
        private System.IO.StreamWriter writer = null;

        private Queue toSendQueue;
        private Queue receivedQueue;
        private Thread connectingThread;
        private Thread sendingThread;
        private Thread receivingThread;
        protected bool connectionTimeout = false;
        public bool ConnectionTimeout
        {
            get { return connectionTimeout; }
        }

        protected bool isAlive = true;

        public WiFiService()
        {
            toSendQueue = new Queue();
            receivedQueue = new Queue();

            receivingThread = new Thread(new ThreadStart(receivingThreadBody));

            sendingThread = new Thread(new ThreadStart(sendingThreadBody));

            connectingThread = new Thread(new ThreadStart(connectingThreadBody));

            receivingThread.Name = "receivingThread";
            sendingThread.Name = "sendingThread";
            connectingThread.Name = "connectingThread";

        }

        protected abstract TcpClient connectSpecific();

        public void connect()
        {
            isAlive = true;
            connectionTimeout = false;
            if(connectingThread.ThreadState != ThreadState.Unstarted)
                connectingThread = new Thread(new ThreadStart(connectingThreadBody));
            connectingThread.Start();
        }

        public void Stop()
        {
            isAlive = false;
        }

        public bool isConnected()
        {
            return client != null;
        }

        public GamePacket receive()
        {
            if (receivedQueue.Count > 0)
                return (GamePacket)receivedQueue.Dequeue();
            else
                return null;
        }

        public void send(GamePacket packet)
        {
            if (packet != null)
                toSendQueue.Enqueue(packet);
        }

        public static String getLocalIP()
        {
            IPHostEntry host;
            string localIP = "xxx.xxx.xxx.xxx";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private void receivingThreadBody()
        {
            while (binaryReader != null && client != null && isAlive)
            {
                String serializedPacket = "";
                while (!serializedPacket.EndsWith(Global.END_OF_PACKET) && isAlive)
                {
                    if (stream.DataAvailable)
                    {
                        serializedPacket += binaryReader.ReadChar();
                    }
                    else
                        Thread.Sleep(200);
                }

                if (isAlive)
                {
                    System.Diagnostics.Debug.WriteLine("packet received => deserialization");
                    serializedPacket = serializedPacket.Replace(Global.END_OF_PACKET, "");
                    System.Diagnostics.Debug.WriteLine(serializedPacket);
                    receivedQueue.Enqueue(GamePacketSerialization.deserialize(serializedPacket));
                    System.Diagnostics.Debug.WriteLine("queue size: " + receivedQueue.Count);
                }
            }
        }

        private void sendingThreadBody()
        {
            while (binaryWriter != null && client != null && isAlive)
            {
                if (toSendQueue.Count > 0)
                {
                    GamePacket packet = (GamePacket)toSendQueue.Dequeue();
                    String serializedPacket = GamePacketSerialization.serialize(packet) + Global.END_OF_PACKET;
                    for (int i = 0; i < serializedPacket.Length; i++)
                        binaryWriter.Write(serializedPacket[i]);
                }

                Thread.Sleep(200);
            }
        }

        private void connectingThreadBody()
        {
            client = connectSpecific();
            if (client == null)
                return;

            stream = client.GetStream();
            binaryWriter = new System.IO.BinaryWriter(stream);
            binaryReader = new System.IO.BinaryReader(stream);
            writer = new System.IO.StreamWriter(stream);
            reader = new System.IO.StreamReader(stream);

            binaryWriter.Write(Global.DEVICE_TYPE_WINDOWS);
            Global.OPPONENT_DEVICE_TYPE = binaryReader.ReadByte();

            if (receivingThread.ThreadState != ThreadState.Unstarted)
                receivingThread = new Thread(new ThreadStart(receivingThreadBody));
            if (sendingThread.ThreadState != ThreadState.Unstarted)
                sendingThread = new Thread(new ThreadStart(sendingThreadBody));

            receivingThread.Start();
            sendingThread.Start();
        }
    }
}
