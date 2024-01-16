using Helpers;
using Logging;
using System.IO.Ports;
using static System.Console;

namespace SerialSingleReceiver;

internal class App
{
    private readonly SimpleLogger logger;
    private string filePath;

    internal App() 
    {
        logger = new(nameof(App), true, true, commonColor: ConsoleColor.Green);
    }

    internal void Run(int baudRate, string fileName = "record.txt")
    {
        SerialPort uart = new();
        string message;

        try
        {
            if (baudRate <= 0) throw new ArgumentException("Incorrect baud rate.");

            uart = new("COM1", baudRate, Parity.None, 8, StopBits.One) { Handshake = Handshake.None };
            uart.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            while (SerialPort.GetPortNames().Length < 1)
            {
                WriteLine("No available ports\nPress any key to refresh or 0 to exit...\n");
                if (int.TryParse(ReadLine(), out int zero) && zero == 0)
                {
                    logger.Log("App close.");
                    Environment.Exit(0);
                }
            }

            SerialPort.GetPortNames().ToList().ForEach(p => WriteLine(p));

            while (true)
            {
                WriteLine("Select ordered number of COM port: ");

                if (int.TryParse(ReadLine(), out int port))
                {
                    if ((port < 1) || (port > SerialPort.GetPortNames().Length))
                    {
                        logger.Log($"Incorrect number: {port}", LogLevels.Warning);
                        continue;
                    }

                    uart.PortName = SerialPort.GetPortNames()[port - 1];

                    break;
                }

                logger.Log("Incorrect symbol", LogLevels.Warning);
            }

            while (true)
            {
                try
                {
                    WriteLine("Folder: ");
                    filePath = FileWorker.GetNextPath(Environment.CurrentDirectory + $"\\{DateTime.Today:d}\\" + ReadLine()?.Trim() + "\\" + fileName);
                    break;
                }
                catch (Exception ex)
                {
                    WriteLine(ex.Message, ConsoleColor.Red);
                }
            }

            uart.DiscardInBuffer();
            uart.DiscardOutBuffer();
            uart.Open();
            ReadLine();
        }
        catch (Exception ex)
        {
            logger.Log(ex.Message, LogLevels.Error);
            ReadKey();
        }
        finally
        {
            uart.Close();
            uart.Dispose();
            logger.Log("App close.");
        }
    }

    void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        string receive = ((SerialPort)sender).ReadExisting();

        try
        {
            File.AppendAllText(filePath, receive);
            WriteLine(receive);
        }
        catch (Exception ex)
        {
            logger.Log(ex.Message, LogLevels.Error);
        }
    }
}
