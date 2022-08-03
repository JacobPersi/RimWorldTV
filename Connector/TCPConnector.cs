using System;
using System.Net.Sockets;
using RimWorld;
using Verse;

namespace RimWorldTV {
    public class TcpConnector {
        public ConnectorStatus Status { get; private set; }

        private string Hostname;
        private uint Port;
        private TcpClient Client;
        private NetworkStream Stream;

        public TcpConnector(string hostname, uint port) {
            Hostname = hostname;
            Port = port;
            Status = ConnectorStatus.Uninitialized;
        }

        public void Connect() {
            ModService.Instance.Alert("Notification.Attempting");
            ModService.Instance.Logger.Trace("Attempting connection...");
            Client = new TcpClient(Hostname, (int)Port);
            Stream = Client.GetStream();
            if (Client.Connected) {
                ModService.Instance.Alert("Notification.Connected");
                ModService.Instance.Logger.Trace("Connected!");
                Status = ConnectorStatus.Connected;
            }
        }

        public void Send(string message) {
            try {
                byte[] data = EncodeMessage(message);

                // Todo: Adds a null byte, not sure if this is required. 
                byte[] data2 = new byte[data.Length + 1];
                data.CopyTo(data2, 0);
                data2[data.Length] = 0;

                ModService.Instance.Logger.Trace($"Sent: {message}");
                Stream.Write(data, 0, data.Length);
            }
            catch (System.IO.IOException) {
                Status = ConnectorStatus.Failure;
            }
        }

        public string Recieve() {
            string result = String.Empty;
            byte[] data = new byte[1024];
            int bytesRead = 0;
            try {
                bytesRead = Stream.Read(data, 0, data.Length);
                result = DecodeMessage(data, bytesRead);
            }
            catch (System.IO.IOException) {
                Status = ConnectorStatus.Failure;
            }
            ModService.Instance.Logger.Trace($"Recieved: {result}");
            return result;
        }

        public void Disconnect() {
            Status = ConnectorStatus.Disconnected;
            ModService.Instance.Logger.Trace("Disconnected.");
        }

        private byte[] EncodeMessage(string message) {
            return System.Text.Encoding.ASCII.GetBytes(message);
        }

        private string DecodeMessage(byte[] data, int length) {
            return System.Text.Encoding.ASCII.GetString(data, 0, length);
        }
    }
}