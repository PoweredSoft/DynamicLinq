using PoweredSoft.DynamicLinq.Dal.Pocos;
using Microsoft.EntityFrameworkCore;
using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace PoweredSoft.DynamicLinq.Dal
{
    public class BlogCoreContext : DbContext
    {
        public BlogCoreContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected BlogCoreContext()
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
