using DataHandler;

var options = Environment.GetCommandLineArgs();

if (options is null || options.Length < 2) Environment.Exit(-1);

new App().Run(options[1]);