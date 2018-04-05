using PoweredSoft.DynamicLinq.Dal.Pocos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal.Configurations
{
    public class AuthorConfiguration : EntityTypeConfiguration<Author>
    {
        public AuthorConfiguration() : this("dbo")
        {

        }

        public AuthorConfiguration(string schema)
        {
            ToTable("Author", schema);
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.FirstName).HasColumnType("nvarchar").HasMaxLength(50).IsRequired();
            Property(t => t.LastName).HasColumnType("nvarchar").HasMaxLength(50).IsRequired();

            HasOptional(t => t.Website).WithMany(t => t.Authors).HasForeignKey(t => t.WebsiteId).WillCascadeOnDelete(false);
        }
    }

    public class PostConfiguration : EntityTypeConfiguration<Post>
    {
        public PostConfiguration() : this("dbo")
        {

        }

        public PostConfiguration(string schema)
        {
            ToTable("Post", schema);
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.AuthorId).HasColumnName("AuthorId").HasColumnType("bigint").IsRequired();
            Property(t => t.Title).HasColumnName("Title").HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            Property(t => t.Content).HasColumnName("Content").HasColumnType("nvarchar(max)").IsRequired();
            Property(t => t.CreateTime).HasColumnName("CreateTime").HasColumnType("datetimeoffset").IsRequired();
            Property(t => t.PublishTime).HasColumnName("PublishTime").HasColumnType("datetimeoffset").IsOptional();

            HasRequired(t => t.Author).WithMany(t => t.Posts).HasForeignKey(t => t.AuthorId).WillCascadeOnDelete(false);
        }
    }

    public class CommentConfiguration : EntityTypeConfiguration<Comment>
    {
        public CommentConfiguration() : this("dbo")
        {

        }

        public CommentConfiguration(string schema)
        {
            ToTable("Comment", schema);
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.PostId).HasColumnName("PostId").HasColumnType("bigint").IsRequired();
            Property(t => t.DisplayName).HasColumnName("DisplayName").HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            Property(t => t.Email).HasColumnName("Email").HasColumnType("nvarchar").IsOptional();
            Property(t => t.CommentText).HasColumnName("CommentText").HasColumnType("nvarchar").HasMaxLength(255).IsOptional();

            HasRequired(t => t.Post).WithMany(t => t.Comments).HasForeignKey(t => t.PostId).WillCascadeOnDelete(false);
        }
    }

    public class CommentLikeConfiguration : EntityTypeConfiguration<CommentLike>
    {
        public CommentLikeConfiguration() : this("dbo")
        {
        }

        public CommentLikeConfiguration(string schema)
        {
            ToTable("CommentLike", schema);
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.CommentId).HasColumnName("CommentId").HasColumnType("bigint").IsRequired();
            Property(t => t.CreateTime).HasColumnName("CreateTime").HasColumnType("datetimeoffset").IsRequired();

            HasRequired(t => t.Comment).WithMany(t => t.CommentLikes).HasForeignKey(t => t.CommentId).WillCascadeOnDelete(false);
        }
    }

    public class WebsiteConfiguration : EntityTypeConfiguration<Website>
    {
        public WebsiteConfiguration() : this("dbo")
        {

        }

        public WebsiteConfiguration(string schema)
        {
            ToTable("Website", schema);
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Title).HasColumnName("Title").HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            Property(t => t.Url).HasColumnName("Url").HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
        }
    }
}
