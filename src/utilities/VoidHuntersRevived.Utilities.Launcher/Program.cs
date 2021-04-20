using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using VoidHuntersRevived.Utilities.Launcher.Services;

namespace VoidHuntersRevived.Utilities.Launcher
{
    class Program
    {
        static int Main(string[] args)
        {
            var command = new RootCommand();
            command.AddArgument(new Argument<String>("type", "The application type to launch."));
            command.AddArgument(new Argument<String>("rid", () => RuntimeIdentifierService.Get(), "The local Runtime Identifier. If none is defined one will be determined based on the current system."));
            command.AddArgument(new Argument<String>("version", () => ReleaseService.GetLatest()?.Version ?? "0.0.0", "The version in question"));
            command.AddOption(new Option<Boolean>("--update", "Download the requested version."));
            command.AddOption(new Option<Boolean>("--launch", "Whether or not the requested version should be launched."));
            command.AddOption(new Option<Boolean>("--check", "Determin whether or not an update is available."));

            command.Handler = CommandHandler.Create<String, String, String, Boolean, Boolean, Boolean, IConsole>((type, rid, version, update, launch, check, console) =>
            {
                try
                {
                    if (update || (launch && !ReleaseService.HasLocal(type, version)))
                        ReleaseService.GetRemote(console, type, version, rid).Download(console);

                    if (launch)
                        ReleaseService.LaunchLocal(console, type, version);

                    if (check)
                        console.Out.Write((!ReleaseService.HasLocal(type, ReleaseService.GetLatest().Version)).ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            });

            return command.Invoke(args);
        }
    }
}
