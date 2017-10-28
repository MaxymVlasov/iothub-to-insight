namespace IoTLoadEmulator
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// The for each.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// The get integer.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetInt(this byte[] bytes, int offset)
        {
            var arr = new int[1];
            Buffer.BlockCopy(bytes, offset, arr, 0, 1);
            return arr[0];
        }
    }
}