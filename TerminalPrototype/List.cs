namespace Terminal
{
    public static class List
    {
        public static string? Run(string[]? args)
        {
            var files = Directory.GetFiles(GlobalState.CurrentDirectory);
            var directories = Directory.GetDirectories(GlobalState.CurrentDirectory);

            var elements = directories.Concat(files).OrderBy(x => x).Select(x => x.Replace("\\\\", "\\")).ToArray();
            elements = elements.Select(x => x[(GlobalState.CurrentDirectory.Length+1)..]).ToArray();

            return $" {string.Join("\n ", elements)}";
        }
    }
}
