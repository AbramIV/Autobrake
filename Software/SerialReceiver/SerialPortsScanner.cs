using Logging;
using System.IO.Ports;

namespace SerialMultipleReceiver;

internal class SerialPortsScanner : IDisposable
{
    private readonly System.Timers.Timer limiter;
    private readonly SimpleLogger logger;

    private readonly string request;
    private readonly string response;

    private bool isAwaiting = false;
    private bool isTimeout = false;
    private bool isCorrect = false;

    internal SerialPortsScanner(string initRequest = "INIT$", string awaitedResponse = "OK$", int timeout = 1000)
    {
        request = initRequest;
        response = awaitedResponse;
        limiter = new(timeout) { AutoReset = false };
        limiter.Elapsed += Limiter_Elapsed;
        logger = new(nameof(SerialPortsScanner), commonColor: ConsoleColor.Green);
    }

    internal IEnumerable<SerialPort?>? Scan(int speed)
    {
        List<SerialPort> ports = new();

        foreach (var name in SerialPort.GetPortNames())
        {
            SerialPort port = new(name, speed, Parity.None, 8, StopBits.One);

            port.DataReceived += SerialPort_DataReceived;

            isAwaiting = true;

            try
            {
                port.Open();
                logger.Log($"{name}: opened.", LogLevels.Debug);
                Thread.Sleep(3000);
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.Write(request);
                logger.Log($"{name}: init request sent.", LogLevels.Debug);

                limiter.Start();
                logger.Log($"{name}: awaiting response.", LogLevels.Debug);

                while (isAwaiting);

                if (isTimeout)
                    throw new TimeoutException("Response timeout.");

                if (isCorrect)
                    logger.Log($"{name} connected.");
                else
                    throw new ArgumentException("Response is incorrect.");
            }
            catch (ArgumentException ex)
            {
                port.Close();
                logger.Log($"{name}: {ex.Message}", LogLevels.Error);
                continue;
            }
            catch (TimeoutException ex)
            {
                port.Close();
                logger.Log($"{name}: {ex.Message}", LogLevels.Warning);
                continue;
            }
            catch (Exception ex)
            {
                port.Close();
                logger.Log($"{name}: {ex.Message}", LogLevels.Error);
                continue;
            }
            finally
            {
                if (limiter.Enabled) limiter?.Stop();
                isAwaiting = false;
                isTimeout = false;
                isCorrect = false;
            }

            port.DataReceived -= SerialPort_DataReceived;
            ports.Add(port);
        }

        logger.Log($"Scanning finished.");
        return ports.ToArray();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        limiter?.Stop();
        var name = ((SerialPort)sender).PortName;
        var data = ((SerialPort)sender).ReadExisting();
        logger.Log($"{name} response: {data}.", LogLevels.Debug);
        isCorrect = data.Trim().Equals(response);
        isAwaiting = false;
    }

    private void Limiter_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        limiter?.Stop();
        isAwaiting = false;
        isTimeout = true;
    }

    public void Dispose()
    {
        limiter?.Close();
        limiter?.Dispose();
    }
}