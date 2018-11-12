using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace TestEFCoreCache
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }


    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BloggingContext>
    {
        public BloggingContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BloggingContext>();
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            var connection = @"Server=(local);Database=TestEFCoreCache;Trusted_Connection=True;ConnectRetryCount=0";
            builder.UseSqlServer(connection);
            return new BloggingContext(builder.Options);
        }
    }
}