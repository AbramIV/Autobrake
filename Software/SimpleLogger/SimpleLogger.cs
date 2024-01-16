namespace Logging;

public class SimpleLogger
{
    private readonly string name;
    private readonly string path;
    private readonly ConsoleColor common;

    public bool WriteToConsole { get; set; }
    public bool WriteToFile { get; set; }

    public SimpleLogger(string category, bool writeToFile = true, bool writeToConsole = false, ConsoleColor commonColor = ConsoleColor.Gray)
    {
        name = category;
        WriteToFile = writeToFile;
        WriteToConsole = writeToConsole;
        path = Environment.CurrentDirectory + $@"\logs.log";
        common = commonColor;
    }

    public async void Log(string message, LogLevels level = LogLevels.Info)
    {
        if (WriteToFile) await File.AppendAllTextAsync(path, $"{DateTime.Now} | {name} | {level} | {message}");

        if (!WriteToConsole) return;

        Console.ForegroundColor = level switch
        {
            LogLevels.Debug => ConsoleColor.Magenta,
            LogLevels.Warning => ConsoleColor.Yellow,
            LogLevels.Error => ConsoleColor.Red,
            LogLevels.Fatal => ConsoleColor.DarkRed,
            _ => common,
        };

        Console.WriteLine($"{DateTime.Now} | {name} | {level} | {message}");
    }
}