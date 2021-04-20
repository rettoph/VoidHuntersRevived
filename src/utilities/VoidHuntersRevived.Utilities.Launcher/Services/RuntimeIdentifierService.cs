using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Utilities.Launcher.Services
{
    public static class RuntimeIdentifierService
    {
        public static String Get()
        {
            var rid = "unknown";
            var osInfo = System.Environment.OSVersion;


            // https://docs.microsoft.com/en-us/dotnet/api/system.platformid?view=net-5.0
            switch (osInfo.Platform)
            {
                case PlatformID.Win32NT:
                    switch (osInfo.Version.Major)
                    {
                        case 10:
                            rid = "win10";
                            break;
                    }
                    break;
            }

            switch (RuntimeInformation.OSArchitecture)
            {
                case Architecture.X86:
                    rid += "-x86";
                    break;
                case Architecture.X64:
                    rid += "-x64";
                    break;
                case Architecture.Arm:
                    rid += "-arm";
                    break;
                case Architecture.Arm64:
                    rid += "-arm64";
                    break;
            }


            return rid;
        }
    }
}
