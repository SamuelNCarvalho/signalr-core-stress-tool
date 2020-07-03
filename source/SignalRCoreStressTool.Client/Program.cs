using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRCoreStressTool.Client
{
    class Program
    {
        /// <summary>
        /// Start method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Running loadtest. Press enter key to exit...");

            int START_NUMBER_OF_CLIENTS = 50;
            int MAX_NUMBER_OF_CLIENTS = 500;
            int STEP_NUMBER_OF_CLIENTS = 10;
            int STEP_TIME = 2;
            int MESSAGE_INTERVAL = 10;
            int RUN_DURATION = 600;

            ConnectionManager instance = new ConnectionManager();

            instance.Init("http://localhost:5000/hubs", async (connection) => {
                await connection.InvokeAsync("Hello");
                await Task.Delay(1000);
                await connection.InvokeAsync("Bye");
            });

            instance.GenerateLoad(
                START_NUMBER_OF_CLIENTS,
                MAX_NUMBER_OF_CLIENTS,
                STEP_NUMBER_OF_CLIENTS,
                STEP_TIME,
                MESSAGE_INTERVAL,
                RUN_DURATION);

            Console.ReadLine();
        }
    }
}
