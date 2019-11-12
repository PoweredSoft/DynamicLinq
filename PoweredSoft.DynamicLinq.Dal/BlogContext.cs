using PoweredSoft.DynamicLinq.Dal.Configurations;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal
{

    public class BlogContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }

        static BlogContext()
        {
            Database.SetInitializer<BlogContext>(new DropCreateDatabaseAlways<BlogContext>());
        }

        public BlogContext()
        {

        }

        public BlogContext(string connectionString) : base(connectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new AuthorConfiguration());
            modelBuilder.Configurations.Add(new CommentConfiguration());
            modelBuilder.Configurations.Add(new PostConfiguration());
            modelBuilder.Configurations.Add(new WebsiteConfiguration());
            modelBuilder.Configurations.Add(new CommentLikeConfiguration());
            modelBuilder.Configurations.Add(new UniqueConfiguration());
        }
    }
}
