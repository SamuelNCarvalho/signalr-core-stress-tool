using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRCoreStressTool.Client
{
    public class ConnectionManager
    {
        /// <summary>
        /// Test Url
        /// </summary>
        private string Url { get; set; }

        /// <summary>
        /// Connection config
        /// </summary>
        private Action<HubConnection> ConnectionConfig { get; set; }

        /// <summary>
        /// Proccess interval
        /// </summary>
        private int MessagingInterval = 30;

        /// <summary>
        /// Parameter to stop execution
        /// </summary>
        public bool IsStopped = false;

        /// <summary>
        /// List o concurrent connections
        /// </summary>
        private ConcurrentBag<SignalRHubConnection> connections = new ConcurrentBag<SignalRHubConnection>();

        /// <summary>
        /// Constructor method
        /// </summary>
        public ConnectionManager()
        {
            Console.WriteLine("Instantiating ConnectionManager");
        }

        /// <summary>
        /// Initialize the ConnectionManager
        /// </summary>
        /// <param name="url">The SignalR Hub url to connect</param>
        /// <param name="connection">Connection config</param>
        public void Init(string url, Action<HubConnection> connectionConfig = null)
        {
            Url = url;
            ConnectionConfig = connectionConfig;
            Console.WriteLine("Init : \n\t{0}", Url);
        }

        /// <summary>
        /// Generates load as per the inputs specified. Init should be called prior to this call.
        /// </summary>
        /// <param name="startNumberOfClients">Initial number of client connections to start the load simulation</param>
        /// <param name="maxNumberOfClients">Final maximum number of client connections to simulate</param>
        /// <param name="stepClientSize">Step size for increasing the client connections</param>
        /// <param name="stepTimeInSeconds">Time interval between increasing client connections based on stepClientSize parameter</param>
        /// <param name="messagingInterval">A time interval in seconds to say how often the Hub function should be invoked</param>
        /// <param name="loadTestDurationInSeconds">The total duration for running the test</param>
        public async void GenerateLoad(
            int startNumberOfClients,
            int maxNumberOfClients,
            int stepClientSize,
            int stepTimeInSeconds,
            int messagingInterval,
            int loadTestDurationInSeconds)
        {
            MessagingInterval = messagingInterval;

            Console.WriteLine(string.Format("GenerateLoad called at {0}", DateTime.Now));
            GenerateLoad(startNumberOfClients, maxNumberOfClients, stepClientSize, stepTimeInSeconds);

            await Task.Delay(loadTestDurationInSeconds * 1000);
            Stop();
        }

        /// <summary>
        /// Method to interrupt and stop the load test
        /// </summary>
        public void Stop()
        {
            Console.WriteLine("ConnectionManager.Stop called");
            IsStopped = true;
        }

        /// <summary>
        /// Start the test
        /// </summary>
        /// <param name="startNumberOfClients">Initial number of client connections to start the load simulation</param>
        /// <param name="maxNumberOfClients">Final maximum number of client connections to simulate</param>
        /// <param name="stepClientSize">Step size for increasing the client connections</param>
        /// <param name="stepTimeInSeconds">Time interval between increasing client connections based on stepClientSize parameter</param>
        /// <returns></returns>
        private async void GenerateLoad(
            int startNumberOfClients,
            int maxNumberOfClients,
            int stepClientSize,
            int stepTimeInSeconds)
        {
            ThreadPool.SetMinThreads(maxNumberOfClients, 2);

            Console.WriteLine("Creating {0} initial connections at {1}", startNumberOfClients, DateTime.Now);
            CreateConnections(startNumberOfClients);

            while (!IsStopped && stepClientSize > 0 && connections.Count + stepClientSize <= maxNumberOfClients)
            {
                Console.WriteLine("Total connections: {0}", connections.Count);
                Console.WriteLine("Wait for {0} seconds before adding more connections", stepTimeInSeconds);
                await Task.Delay(stepTimeInSeconds * 1000);

                Console.WriteLine("Creating {0} step connections at {1}", stepClientSize, DateTime.Now);
                CreateConnections(stepClientSize);
            }
        }

        /// <summary>
        /// Create the connections
        /// </summary>
        /// <param name="numberOfClients">New connections quantity</param>
        private void CreateConnections(int numberOfClients)
        {
            Parallel.For(0, numberOfClients, (i) =>
            {
                try
                {
                    if (IsStopped)
                    {
                        return;
                    }

                     SignalRHubConnection connection = new SignalRHubConnection(this, Url);

                    string connectionId = connection.ConnectionId;

                    Console.WriteLine(string.Format("Connection created : {0} ({1})", connection.ConnectionId, Thread.CurrentThread.GetHashCode()));

                    if (!IsStopped)
                    {
                        connections.Add(connection);

                        if (ConnectionConfig != null)
                        {
                            if (MessagingInterval > 0)
                            {
                                connection.Invoke(ConnectionConfig, MessagingInterval);
                            }
                            else
                            {
                                connection.Invoke(ConnectionConfig);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("===== Error in Create Connection =====");
                    do
                    {
                        Console.WriteLine(string.Format("{0}\n{1}", exc.Message, exc.StackTrace));
                        exc = exc.InnerException;
                    }
                    while (exc != null);
                }
            });
        }
    }
}
