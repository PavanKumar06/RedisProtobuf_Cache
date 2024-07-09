using Microsoft.Extensions.Caching.Distributed;
using ProtoBuf;
using Redis.Data;
using System.Text.Json;

namespace Redis.Helpers
{
	//For this to work execute this ->
	//docker run --name my-redis -p 5002:6379 -d redis
	//docker exec -it my-redis sh
	//redis-cli
	//Redis cmds ->
	//ping, select 0, dbsize, scan 0, hgetall <key>
	//To stop ->
	//exit, exit, docker stop <id>, docker rm <id>
	public static class DistributedCacheHelper
	{
		//This is an extension method hence we have "this" in front. This way if we have an instance of type IDistributedCache
		//Using "." you can access SetRecordAsync.
		//recordId is the KEY, must be unique.
		//Record is the data stored in cache.
		public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T record, 
			TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
		{
			var options = new DistributedCacheEntryOptions();

			options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
			options.SlidingExpiration = unusedExpireTime;

			//var cachedValue = JsonSerializer.Serialize(record);

			//await cache.SetStringAsync(recordId, cachedValue, options);
            
			using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, record);
                var cachedValue = memoryStream.ToArray();
                await cache.SetAsync(recordId, cachedValue, options);
            }
        }

		public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
		{
			//var cachedValue = await cache.GetStringAsync(recordId);

			var cachedValue = await cache.GetAsync(recordId);

			if (cachedValue == null)
			{
				return default(T);
			}

            //return JsonSerializer.Deserialize<T>(cachedValue);

            using (var memoryStream = new MemoryStream(cachedValue))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }
	}
}
