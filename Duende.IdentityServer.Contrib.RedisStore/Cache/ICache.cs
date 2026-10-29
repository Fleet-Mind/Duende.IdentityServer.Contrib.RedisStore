using System;
using System.Threading.Tasks;

namespace Duende.IdentityServer.Contrib.RedisStore.Cache
{
  /// <summary>
  /// Abstract interface to model data caching.
  /// Duende IdentityServer 8 removed its own ICache&lt;T&gt; (it now uses HybridCache),
  /// so this library defines the interface it needs for RedisCache and CachingProfileService.
  /// </summary>
  /// <typeparam name="T">The data type to be cached.</typeparam>
  public interface ICache<T> where T : class
  {
    /// <summary>
    /// Gets the cached data based upon a key index.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The cached item, or <c>null</c> if no item matches the key.</returns>
    Task<T> GetAsync(string key);

    /// <summary>
    /// Caches the data based upon a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="item">The item.</param>
    /// <param name="expiration">The expiration.</param>
    Task SetAsync(string key, T item, TimeSpan expiration);

    /// <summary>
    /// Gets the cached data, or calls <paramref name="get"/> and caches the result if it is missing.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="duration">The expiration.</param>
    /// <param name="get">Loads the item when it is not in the cache.</param>
    Task<T> GetOrAddAsync(string key, TimeSpan duration, Func<Task<T>> get);

    /// <summary>
    /// Removes the cached data based upon a key index.
    /// </summary>
    /// <param name="key">The key.</param>
    Task RemoveAsync(string key);
  }
}
