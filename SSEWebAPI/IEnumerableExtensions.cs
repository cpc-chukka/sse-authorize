namespace SSEWebAPI
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> coll, Action<T> action)
        {
            foreach (var item in coll)
            {
                action(item);
            }
        }
    }
}
