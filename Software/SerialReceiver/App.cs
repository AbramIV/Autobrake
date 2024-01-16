using Logging;
using Helpers;
using System.IO.Ports;
using System.Reflection;
using static System.Console;

namespace SerialMultipleReceiver;

internal class App : IDisposable
{
    private readonly SimpleLogger logger;
    private IEnumerable<SerialPort?>? ports;
    private IEnumerable<Task> tasks;
    private readonly int portSpeed;

    internal App(int baudRate)
    {
        Title = Assembly.GetCallingAssembly().GetName().Name ?? "SerialReceiver";
        logger = new(nameof(App), commonColor: ConsoleColor.Green);
        portSpeed = baudRate;
    }

    internal void Run()
    {
        try
        {
            if (portSpeed <= 0) throw new ArgumentException("Incorrect baud rate.");

            while (true)
            {
                WriteLine("Serial ports scanning...");

                using var scanner = new SerialPortsScanner();
                ports = scanner.Scan(portSpeed);

                if (ports is null || !ports.Any())
                {
                    WriteLine("No available ports\nPress any key to refresh or 0 to exit...\n");

                    if (int.TryParse(ReadLine(), out int exit) && exit == 0) Environment.Exit(0);

                    continue;
                }

                break;
            }

            foreach (var port in ports) logger.Log($"{port.PortName} is ready.");

            string path = $@"{Environment.CurrentDirectory}\{DateTime.Today:d}\";

            WriteLine($"Data folder: {path}");
            WriteLine("Data receiving...");

            tasks = ports.Select(port => Task.Factory.StartNew(() =>
                new SerialPortsHandler(port, 500, FileWorker.GetNextPath($@"{path}record_({port?.PortName}).txt")).Handle()));

            Task.WaitAll(tasks.ToArray());
        }
        catch (Exception ex)
        {
            logger.Log(ex.ToString(), LogLevels.Error);
        }
    }

    public void Dispose()
    {
        if (ports is not null)
        {
            foreach (var port in ports)
            {
                port?.Close();
                port?.Dispose();
            }
        }

        logger.Log("App close.", LogLevels.Debug);
    }
}