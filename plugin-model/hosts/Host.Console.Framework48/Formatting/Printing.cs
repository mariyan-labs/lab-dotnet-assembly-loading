namespace Host.Console.Framework48.Formatting;

public static class Printing
{
    extension(string value)
    {
        public void Print()
        {
            System.Console.WriteLine(value);
        }

        public void WriteTo(string path)
        {
            File.WriteAllText(path, value);
        }
    }
}