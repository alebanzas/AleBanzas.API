using System;

namespace AB.Data
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="id">The identifier for the <typeparamref name="T"/> instance.</param>
        /// <returns>The object in the cache or <see langword="default"/> where the object was not available.</returns>
        T Get<T>(string id);

        /// <summary>
        /// Inserts or override an item into the cache with a cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to put.</typeparam>
        /// <param name="id">The identifier for the <typeparamref name="T"/> instance.</param>
        /// <param name="value">The object to be inserted into or overriden in the cache.</param>
        /// <param name="relativeExpiration">The time at which the inserted object expires and is removed from the cache (it is relative to <see cref="DateTime.UtcNow"/>).</param>
        /// <returns>The object inserted in the cache or <see langword="default"/> where the <paramref name="id"/> is invalid.</returns>
        T Put<T>(string id, T value, TimeSpan relativeExpiration);

        /// <summary>
        /// Inserts or override an item into the cache with a cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to put.</typeparam>
        /// <param name="id">The identifier for the <typeparamref name="T"/> instance.</param>
        /// <param name="value">The object to be inserted into or overriden in the cache.</param>
        /// <param name="slidingExpiration">The interval between the time the inserted object is last accessed and the time at which that object expires.</param>
        /// <returns>The object inserted in the cache or <see langword="default"/> where the <paramref name="id"/> is invalid.</returns>
        T PutWithSliding<T>(string id, T value, TimeSpan slidingExpiration);

        /// <summary>
        /// Removes the specified item from the application's cache object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="id">The identifier for the <typeparamref name="T"/> instance was inserted.</param>
        void Evict<T>(string id);
    }
}
