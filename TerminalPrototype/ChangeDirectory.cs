using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal;

namespace TerminalPrototype
{
    public static class ChangeDirectory
    {
        public static string? Run(string[]? args)
        {
            if (args == null || !args.Any()) return null;
            var path = args[0];

            if (path == "..")
                GlobalState.CurrentDirectory = Directory.GetParent(GlobalState.CurrentDirectory).FullName;
            else if (Directory.Exists(path))
                GlobalState.CurrentDirectory = new DirectoryInfo(path).FullName;
            else
            {
                var partPath = $"{GlobalState.CurrentDirectory}\\{path}";
                if (Directory.Exists(partPath))
                    GlobalState.CurrentDirectory = new DirectoryInfo(partPath).FullName;
            }

            return null;
        }
    }
}
