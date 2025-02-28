using System.Collections.Generic;
using Codice.Client.BaseCommands;

namespace ElectricDrill.SimpleRPGCore.SimpleRpgCore.Utils
{
    /// <summary>
    /// A generic cache class that provides basic caching functionality.
    /// </summary>
    /// <typeparam name="KType">The type of the keys in the cache.</typeparam>
    /// <typeparam name="VType">The type of the values in the cache.</typeparam>
    public class Cache<KType, VType>
    {
        // Dictionary to store the cache items.
        private readonly Dictionary<KType, VType> _cache = new();

        /// <summary>
        /// Sets the value for the specified key in the cache. UPSERT operation.
        /// </summary>
        /// <param name="key">The key of the item to set.</param>
        /// <param name="value">The value to set for the specified key.</param>
        public void Set(KType key, VType value) {
            _cache[key] = value;
        }

        /// <summary>
        /// Invalidates the cache item for the specified key. If key is not found, nothing happens.
        /// </summary>
        /// <param name="key">The key of the item to invalidate.</param>
        public void Invalidate(KType key) {
            _cache.Remove(key);
        }

        /// <summary>
        /// Invalidates all items in the cache.
        /// </summary>
        public void InvalidateAll() {
            _cache.Clear();
        }

        /// <summary>
        /// Gets the value for the specified key from the cache.
        /// If the key is not found, a KeyNotFoundException is thrown.
        /// </summary>
        /// <param name="key">The key of the item to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the key is not found in the cache.</exception>
        public VType Get(KType key) {
            return _cache[key];
        }

        /// <summary>
        /// Tries to get the value for the specified key from the cache.
        /// </summary>
        /// <param name="key">The key of the item to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>true if the cache contains an item with the specified key; otherwise, false.</returns>
        public bool TryGet(KType key, out VType value) {
            return _cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Determines whether the cache contains an item with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the cache.</param>
        /// <returns>true if the cache contains an item with the key; otherwise, false.</returns>
        public bool Has(KType key) => _cache.ContainsKey(key);

        /// <summary>
        /// Gets or sets the value for the specified key in the cache.
        /// </summary>
        /// <param name="key">The key of the item to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public VType this[KType key] {
            get => Get(key);
            set => Set(key, value);
        }
    }
}