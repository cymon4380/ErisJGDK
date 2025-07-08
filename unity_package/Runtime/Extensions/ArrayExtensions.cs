namespace ErisJGDK.Base.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns an empty array if provided one is null. Otherwise, returns a provided array itself.
        /// </summary>
        public static T[] ArrayOrEmpty<T>(this T[] array)
        {
            return array ?? new T[0];
        }
    }
}
