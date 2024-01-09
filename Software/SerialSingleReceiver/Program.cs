using SerialSingleReceiver;

int baudRate = 0;
var options = Environment.GetCommandLineArgs();

if (options is not null && options.Length > 1) baudRate = int.Parse(options[1]);

new App().Run(baudRate);