# ASP&#46;NET Core SignalR Stress Tool

A simple stress tool based on [QBurst ASP.NET tool](https://github.com/qburst/signalr-load-simulator) to simulate multiple connections against any SignalR Core hub based application.

## Usage

In `Program.cs` change the connection configuration.

```c#
int START_NUMBER_OF_CLIENTS = 50;
int MAX_NUMBER_OF_CLIENTS = 500;
int STEP_NUMBER_OF_CLIENTS = 10;
int STEP_TIME = 2;
int MESSAGE_INTERVAL = 10;
int RUN_DURATION = 600;

instance.Init("http://localhost:5000/hubs", async (connection) => {
    await connection.InvokeAsync("Hello");
    await Task.Delay(1000);
    await connection.InvokeAsync("Bye");
});
```

## Run
```
dotnet run --project source/SignalRCoreStressTool.Client/SignalRCoreStressTool.Client.csproj
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.