namespace Host.Console.Framework48.Shared;

public static class EnumerationUtils
{
    extension<T>(IEnumerable<T> enumerable)
    {
        public void ForEach(Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
}