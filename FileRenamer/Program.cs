using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace FileRenamer
{
  class Program
  {
    static void Main(string[] args)
    {
      var provider = BuildServices();
      Console.WriteLine("Please Select:");
      Console.WriteLine("1 - DateRenaming");
      Console.WriteLine("2 - Tv Episode Renaming");
      var input = Console.ReadLine();
      if (input == "1")
      {
        var renamer = provider.GetRequiredService<DateRenamer>();
        renamer.Rename(string.Empty);
      }
      else
      {
        Console.WriteLine("Please specify the root file path");
        var path = Console.ReadLine();
        if(!Directory.Exists(path))
        {
          Console.WriteLine("Invalid Path Specified");
          return;
        }
        var renamer = provider.GetRequiredService<TvRenamer>();
        renamer.Rename(path);
      }
      Console.WriteLine("Process Completed");
    }

    static IServiceProvider BuildServices()
    {
      var logFile = "Logs\\FileRenamer.log";
      var services = new ServiceCollection();
      services.AddLogging((builder) =>
      {
        builder.AddSerilog(new LoggerConfiguration()
          .MinimumLevel.Verbose()
          .WriteTo.File(logFile)
          .WriteTo.Console()
          .CreateLogger());
        builder.SetMinimumLevel(LogLevel.Trace);
      });
      services.AddSingleton<DateRenamer>();
      services.AddSingleton<TvRenamer>();
      return services.BuildServiceProvider();
    }
  }
}
