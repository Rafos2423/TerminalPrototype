using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal;

namespace TerminalPrototype
{
    public static class Concatenate
    {
        public static string? Run(string[]? args)
        {
            var defaultText = "No such file or directory";
            if (args == null || !args.Any()) return defaultText;

            try
            {
                var options = args.Where(x => x.StartsWith("-")).Distinct().OrderBy(o => o == "-E").ToList();
                var anotherOptions = args.Except(options).ToList();

                var files = new List<string>();
                foreach (var opt in anotherOptions)
                {
                    if (opt == ">")
                    {
                        files.Add(opt);
                        continue;
                    }

                    var partPath = $"{GlobalState.CurrentDirectory}\\{opt}";
                    if (File.Exists(opt))
                        files.Add(new FileInfo(opt).FullName);
                    else if (File.Exists(partPath))
                        files.Add(new FileInfo(partPath).FullName);
                }

                var readFileGroup = files.TakeWhile(x => x != ">").ToArray();
                var writeFileGroup = files.SkipWhile(x => x != ">").Skip(1).ToArray();

                string text = "";

                if (!readFileGroup.Any())
                {
                    if (writeFileGroup.Any())
                    {
                        var inputText = new StringBuilder();
                        var key = Console.ReadKey();
                        while (key.KeyChar != '\x04')
                        {
                            if (key.Key == ConsoleKey.Enter)
                            {
                                Console.WriteLine();
                                inputText.Append('\n');
                            }

                            else
                                inputText.Append(key.KeyChar);
                            key = Console.ReadKey();
                        }

                        Console.WriteLine();
                        text = inputText.ToString();
                    }
                }
                else
                    text = string.Join("", readFileGroup.Select(File.ReadAllText));

                if (options.Contains("-n") && options.Contains("-b")) options.Remove("-b");

                foreach (var o in options)
                {
                    text = o switch
                    {
                        "-T" => text.Replace("\t", "^|"),
                        "-E" => text.Replace("\n", "\n$"),
                        "-b" => string.Join("\n", text.Split("\n").Select((x, i) => string.IsNullOrEmpty(x) ? x : $"     {i + 1} {x}").ToArray()),
                        "-n" => string.Join("\n", text.Split("\n").Select((x, i) => $"     {i + 1} {x}").ToArray()),
                        "-s" => string.Join("\n", text.Split("\n").Distinct().ToArray()),
                        string => text
                    };
                }

                if (writeFileGroup.Any())
                {
                    foreach (var filename in writeFileGroup)
                    {
                        using var writeText = new StreamWriter(filename);
                        writeText.Write(text);
                    }
                    return null;
                }

                return !string.IsNullOrEmpty(text) ? $"{text}%" : defaultText;
            }
            catch (Exception)
            {
                return defaultText;
            }
        }
    }
}
