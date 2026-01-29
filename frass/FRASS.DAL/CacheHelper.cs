using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace FRASS.DAL
{
	public static class CacheHelper
	{
		public static void Add<T>(T o, string key)
		{
			var cache = MemoryCache.Default;
			CacheItem cacheItem = new CacheItem(key, o);
			CacheItemPolicy policy = new CacheItemPolicy();
			policy.AbsoluteExpiration = DateTime.Now.AddMinutes(10);
			cache.Add(cacheItem, policy);
		}
		public static void Clear(string key)
		{
			var cache = MemoryCache.Default;
			cache.Remove(key);
		}

		public static bool Exists(string key)
		{
			var cache = MemoryCache.Default;
			return cache.Contains(key);
		}

		public static bool Get<T>(string key, out T value)
		{
			var cache = MemoryCache.Default;
			try
			{
				if (!Exists(key))
				{
					value = default(T);
					return false;
				}
				value = (T)cache.Get(key);
			}
			catch
			{
				value = default(T);
				return false;
			}
			return true;
		}

	}
}
