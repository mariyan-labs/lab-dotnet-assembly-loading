namespace Host.Console.Framework48.Shared;

public static class ListUtils
{
    extension<T>(List<T>)
    {
        public static List<T> Empty() => [];
    }
}