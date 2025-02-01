using System.Text;

namespace Terminal
{
    class Programm
    {
        public static void Main()
        {
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            new Process().HandleCommand();
        }
    }
}