using Newtonsoft.Json;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using VoidHuntersRevived.Utilities.Launcher.Enums;
using VoidHuntersRevived.Utilities.Launcher.Models;
using VoidHuntersRevived.Utilities.Launcher.Services;

namespace VoidHuntersRevived.Utilities.Launcher
{
    class Program
    {
        static int Main(string[] args)
        {
            var command = new RootCommand();
            command.AddArgument(new Argument<String>("type", "The application type to launch."));
            command.AddArgument(new Argument<String>("path", () => null, "An optional path override for download directories."));
            command.AddArgument(new Argument<String>("version", () => "latest", "The version in question"));
            command.AddArgument(new Argument<String>("rid", () => RuntimeIdentifierService.Get(), "The local Runtime Identifier. If none is defined one will be determined based on the current system."));
            command.AddOption(new Option<LauncherAction>("--action", "The primary action to take place."));

            command.Handler = CommandHandler.Create<String, String, String, String, LauncherAction, IConsole>((type, path, rid, version, action, console) =>
            {
                try
                {
                    Release release;
                    if (version == "latest")
                    {
                        release = ReleaseService.TryGetLatest(rid, type);
                    }
                    else
                    {
                        var releaseService = new ReleaseService(rid, type);
                        release = releaseService.TryGetVersion(version);
                    }

                    switch (action)
                    {
                        case LauncherAction.Launch:
                            release.Launch(console);
                            break;
                        case LauncherAction.LaunchLocal:
                            release.Launch(console, false);
                            break;
                        case LauncherAction.Update:
                            release.Download(console, path);
                            break;
                        case LauncherAction.Delete:
                            release.Delete(console);
                            break;
                        case LauncherAction.Info:
                            console.Out.Write(JsonConvert.SerializeObject(release));
                            break;
                    }

                }
                catch (Exception e)
                {
                    console.Error.Write(e.Message);
                }
            });

            return command.Invoke(args);
        }
    }
}
