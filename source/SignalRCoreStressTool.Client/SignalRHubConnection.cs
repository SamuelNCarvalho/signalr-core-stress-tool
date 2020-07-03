using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.Threading;

namespace SignalRCoreStressTool.Client
{
    internal class SignalRHubConnection
    {
        /// <summary>
        /// The SignalR Hub connection
        /// </summary>
        private HubConnection Connection = null;

        /// <summary>
        /// Test Url
        /// </summary>
        private string Url { get; set; }

        /// <summary>
        /// The ConnectionManager instance
        /// </summary>
        private ConnectionManager ConnectionManagerInstance { get; set; }

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="instance">The ConnectionManager instance</param>
        /// <param name="url">Test Url</param>
        internal SignalRHubConnection(ConnectionManager instance, string url)
        {
            ConnectionManagerInstance = instance;
            Url = url;
            Initialize();
        }

        /// <summary>
        /// Corrent connection id
        /// </summary>
        internal string ConnectionId
        {
            get { return Connection.ConnectionId; }
        }

        /// <summary>
        /// Invoke hub methods
        /// </summary>
        /// <param name="connectionConfig">The connection config</param>
        internal void Invoke(Action<HubConnection> connectionConfig)
        {
            CallHubMethod(connectionConfig);
        }

        /// <summary>
        /// Invoke hub methods with interval
        /// </summary>
        /// <param name="connectionConfig">The connection config</param>
        /// <param name="interval">Test interval</param>
        internal async void Invoke(Action<HubConnection> connectionConfig, int interval)
        {
            while (!ConnectionManagerInstance.IsStopped)
            {
                CallHubMethod(connectionConfig);

                Debug.WriteLine("Wait for {0} seconds before invoking again on Connection : {1} (2)", interval, this.ConnectionId, Thread.CurrentThread.GetHashCode());
                await Task.Delay(interval * 1000);
            }
        }

        /// <summary>
        /// Call connection config methods
        /// </summary>
        /// <param name="connectionConfig">The connection config</param>
        private void CallHubMethod(Action<HubConnection> connectionConfig)
        {
            try
            {
                if (!ConnectionManagerInstance.IsStopped)
                {
                    if (Connection.State == HubConnectionState.Connected)
                    {
                        connectionConfig(Connection);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Exception '{0}' occured on Connection : {1}", exc.Message, this.ConnectionId);
            }
        }

        /// <summary>
        /// Initialize connection
        /// </summary>
        private async void Initialize()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(Url)
                .Build();

            await Connection.StartAsync();
        }
    }
}