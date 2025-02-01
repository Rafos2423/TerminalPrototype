using System.Text;
using TerminalPrototype;

namespace Terminal
{
    enum Commands { cat, cd, ls }
    public static class GlobalState
    {
        public static string CurrentDirectory 
        { 
            get => Directory.GetCurrentDirectory();
            set => Directory.SetCurrentDirectory(value);
        }
    }

    public class Process
    {
        public string PrintLine() {
            Console.Write($"{GlobalState.CurrentDirectory}>");
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
                    Commands.cat => Concatenate.Run(args),
                    Commands.cd => ChangeDirectory.Run(args),
                    Commands.ls => List.Run(args),
                    _ => $"Command {command} not found"
                };

                if (answer != null)
                    Console.WriteLine(answer);
            }
            else
                Console.WriteLine("Unknown command");

            HandleCommand();
        }
    }
}
