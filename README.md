# ASP&#46;NET Core SignalR Stress Tool

A simple stress tool based on [QBurst ASP.NET tool](https://github.com/qburst/signalr-load-simulator) to simulate multiple connections against any SignalR Core hub based application.

## Usage

In `Program.cs` change the connection configuration.

```c#
instance.Init("http://localhost:5000/hub", async (connection) => {
    await connection.InvokeAsync("Start");
    await Task.Delay(60 * 1000);
    await connection.InvokeAsync("Quit");
});
```

## Run
```
dotnet run --project source/SignalRCoreStressTool.Client/SignalRCoreStressTool.Client.csproj
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.