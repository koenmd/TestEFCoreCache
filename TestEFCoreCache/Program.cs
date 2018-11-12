using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace TestEFCoreCache
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BloggingContext>();
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            var connection = @"Server=(local);Database=TestEFCoreCache;Trusted_Connection=True;ConnectRetryCount=0";
            builder.UseSqlServer(connection);
            //builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            MemoryCache _cache = new MemoryCache(new MemoryCacheOptions() { });
            builder.UseMemoryCache(_cache);

            BloggingContext db = new BloggingContext(builder.Options);

            var qryposts = EF.CompileQuery((BloggingContext dbC, int id)
                => db.Posts.FirstOrDefault(p => p.PostId == id));



            for (int i = 0; i < 100000; i++)
            {
                //var posts1 = db.Posts.Where(p => p.PostId == 2);
                //posts1.ToList();

                //var posts = qryposts(db, 2);
                //posts.ToList();
                var posts = GetPosts(db, _cache);

                Console.WriteLine(i.ToString());

            }
            Console.ReadKey();

        }

        static List<Post> GetPosts(BloggingContext db, MemoryCache cache)
        {
            List<Post> posts = null;
            if (cache != null && !cache.TryGetValue("posts", out posts))
            {
                Console.WriteLine("======================== DB ACCESS ============================");
                // Key not in cache, so get data.
                var posts1 = db.Posts.Where(p => p.PostId == 2);
                posts = posts1.ToList();


                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMilliseconds(100));
                // Keep in cache for this time, reset time if accessed.
                //.SetSlidingExpiration(TimeSpan.FromSeconds(1));
                // Save data in cache.
                cache.Set("posts", posts, cacheEntryOptions);
            }

            return posts;
        }
    }
}
