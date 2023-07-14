﻿using System.Collections.Concurrent;

namespace SlotMachine.Infrastructure
{
    /// <summary>
    /// Lock base on Id
    /// https://github.com/Darkseal/LockProvider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LockProvider<T>
    {
        public static readonly LazyConcurrentDictionary<T, InnerSemaphore> LockDictionary = new LazyConcurrentDictionary<T, InnerSemaphore>();

        public LockProvider() { }

        /// <summary>
        /// Blocks the current thread (according to the given ID) until it can enter the LockProvider
        /// </summary>
        /// <param name="idToLock">the unique ID to perform the lock</param>
        public void Wait(T idToLock)
        {
            LockDictionary.GetOrAdd(idToLock, new InnerSemaphore(1, 1)).Wait();
        }

        /// <summary>
        /// Asynchronously puts thread to wait (according to the given ID) until it can enter the LockProvider
        /// </summary>
        /// <param name="idToLock">the unique ID to perform the lock</param>
        public async Task WaitAsync(T idToLock)
        {
            await LockDictionary.GetOrAdd(idToLock, new InnerSemaphore(1, 1)).WaitAsync();
        }

        public void Release(T idToUnlock)
        {
            if (LockDictionary.TryGetValue(idToUnlock, out var semaphore))
            {
                semaphore.Release();
                if (!semaphore.HasWaiters && LockDictionary.TryRemove(idToUnlock, out semaphore))
                    semaphore.Dispose();
            }
        }
    }

    public class InnerSemaphore : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private int _waiters;

        public InnerSemaphore(int initialCount, int maxCount)
        {
            _semaphore = new SemaphoreSlim(initialCount, maxCount);
            _waiters = 0;
        }

        public void Wait()
        {
            _waiters++;
            _semaphore.Wait();
        }

        public async Task WaitAsync()
        {
            _waiters++;
            await _semaphore.WaitAsync();
        }

        public void Release()
        {
            _waiters--;
            _semaphore.Release();
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
        public bool HasWaiters => _waiters > 0;
        public int WaitersCount => _waiters;
    }

    public class LazyConcurrentDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _concurrentDictionary;

        public LazyConcurrentDictionary()
        {
            _concurrentDictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            var lazyResult = _concurrentDictionary.GetOrAdd(key, k => new Lazy<TValue>(() => value, LazyThreadSafetyMode.ExecutionAndPublication));
            return lazyResult.Value;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            var lazyResult = _concurrentDictionary.GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));
            return lazyResult.Value;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var success = _concurrentDictionary.TryGetValue(key, out var lazyResult);
            value = (success) ? lazyResult.Value : default;
            return success;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            var success = _concurrentDictionary.TryRemove(key, out var lazyResult);
            value = (success) ? lazyResult.Value : default;
            return success;
        }

        public IEnumerable<TKey> Keys { get { return _concurrentDictionary.Keys; } }
    }

}
