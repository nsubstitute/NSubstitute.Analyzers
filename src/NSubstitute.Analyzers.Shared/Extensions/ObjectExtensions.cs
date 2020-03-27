namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Performs unsafe cast from <see cref="source"/> to <see cref="T"/>
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <typeparam name="T">Type to cast to.</typeparam>
        /// <returns>Cast source.</returns>
        public static T Cast<T>(this object source)
        {
            return (T)source;
        }
    }
}