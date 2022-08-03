using System;
using System.ComponentModel;
using RimWorld;
using Verse;


namespace RimWorldTV {
    public class EffectListener {
        public event EffectCommandHandler OnEffect;

        private BackgroundWorker Worker;
        private TcpConnector Connector;
        private const string ResponseText = "{{\"id\":{0},\"status\":{1},\"message\":\"\",\"timeRemaining\":0,\"type\":0}}";
        private string Hostname;
        private uint Port;

        public EffectListener(string hostname, uint port) {
            Hostname = hostname;
            Port = port;
            Connector = new TcpConnector(Hostname, Port);
        }

        public void StartBackgroundListener() {
            if (Worker != null) {
                Worker.CancelAsync();
            }
            Worker = new BackgroundWorker();
            Worker.DoWork += OnWorkerExecute;
            Worker.RunWorkerCompleted += OnWorkerFinished;
            Worker.WorkerSupportsCancellation = true;
            Worker.RunWorkerAsync();
        }

        public void ReportEffectStatus(EffectCommand message, EffectStatus status) {
            string response = string.Format(ResponseText, message.id, (int)status);
            Connector.Send(response);
        }

        private void OnWorkerExecute(object sender, DoWorkEventArgs e) {
            while (Worker.CancellationPending == false) {
                ConnectorStatus connectorStatus = Connector.Status;
                switch (connectorStatus) {
                    case ConnectorStatus.Uninitialized:
                        HandleState_Disconnected();
                        break;
                    case ConnectorStatus.Connected:
                        HandleState_Connected();
                        break;
                    case ConnectorStatus.Disconnected:
                        ModService.Instance.Alert("Notification.Disconnected");
                        HandleState_Disconnected();
                        break;
                    case ConnectorStatus.Failure:
                        ModService.Instance.Alert("Notification.Failure");
                        HandleState_Failure();
                        break;
                    default:
                        break;
                }
            }
        }

        private EffectCommand ParseMessage(string message) {
            EffectCommand effectCommand = null;
            try {
                effectCommand = new EffectCommand(message);
                effectCommand.TryParse();
            }
            catch (Exception ex) {
                ModService.Instance.Logger.Trace($"Unable to parse command: {message} - {ex}");
            }
            return effectCommand;
        }

        private void HandleState_Connected() {
            string message = Connector.Recieve();
            EffectCommand effectCommand = ParseMessage(message);
            BroadcastEffect(effectCommand);
        }

        private void HandleState_Disconnected() {
            Connector.Connect();
        }

        private void HandleState_Failure() {
            Connector = new TcpConnector(Hostname, Port);
        }

        private void BroadcastEffect(EffectCommand effectCommand) {
            if (effectCommand.IsValid)
                OnEffect.Invoke(this, effectCommand);
            else {
                ModService.Instance.Logger.Trace($"Invalid effect command: {effectCommand}");
            }
        }

        private void OnWorkerFinished(object sender, RunWorkerCompletedEventArgs e) {
            System.Threading.Thread.Sleep(7000);
            StartBackgroundListener();
        }

        public ConnectorStatus GetConnectionStatus() {
            if (Connector == null)
                return ConnectorStatus.Uninitialized;
            else
                return Connector.Status;
        }
    }
}