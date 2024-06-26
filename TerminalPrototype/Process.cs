﻿using System.Text;

namespace Terminal
{
    enum Commands { cat, cd, ls }
    public class Process
    {
        public string CurrentDirectory { get; set; }

        public string PrintLine() {
            Console.Write($"{CurrentDirectory}>");
            return Console.ReadLine();
        }

        public void HandleCommand()
        {
            var input = PrintLine();

            var splitted = input.Split(' ');
            var command = splitted[0];
            var args = splitted.Skip(1).ToArray();

            if (!int.TryParse(command, out _) &&
                Enum.TryParse(typeof(Commands), command, out var result))
            {
                var answer = result switch
                {
                    Commands.cat => Concatenate(args),
                    Commands.cd => ChangeDirectory(args),
                    Commands.ls => List(args),
                };

                if (answer != null)
                    Console.WriteLine(answer);
            }
            else
                Console.WriteLine("Unknown command");

            HandleCommand();
        }

        public string? Concatenate(string[]? args)
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

                    var partPath = $"{CurrentDirectory}\\{opt}";
                    if (File.Exists(opt))
                        files.Add(new FileInfo(opt).FullName);
                    else if (File.Exists(partPath))
                        files.Add(new FileInfo(partPath).FullName);
                }             

                var readFileGroup = files.TakeWhile(x => x != ">").ToArray();
                var writeFileGroup = files.SkipWhile(x => x != ">").Skip(1).ToArray();

                string text="";

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
                        string => ""
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

        public string? ChangeDirectory(string[]? args)
        {
            if (args != null && args.Any())
            {
                var path = args[0];

                if (path == "..")
                {
                    CurrentDirectory = Directory.GetParent(CurrentDirectory).FullName;
                    return null;
                }

                if (Directory.Exists(path))
                {
                    CurrentDirectory = new DirectoryInfo(path).FullName;
                    return null;
                }

                var partPath = $"{CurrentDirectory}\\{path}";
                if (Directory.Exists(partPath))
                {
                    CurrentDirectory = new DirectoryInfo(partPath).FullName;
                    return null;
                }
            }
            return null;
        }

        public string? List(string[]? args)
        {
            var files = Directory.GetFiles(CurrentDirectory);
            var directories = Directory.GetDirectories(CurrentDirectory);

            var elements = directories.Concat(files).OrderBy(x => x).ToArray();

            elements = elements.Select(x => x[(CurrentDirectory.Length + 1)..]).ToArray();

            return $" {string.Join("\n ", elements)}";
        }

        public void Start()
        {
            CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            HandleCommand();
        }
    }
}
