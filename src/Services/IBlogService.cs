﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Miniblog.Core.Models;

namespace Miniblog.Core.Services
{
    public interface IBlogService
    {
        Task<IEnumerable<Post>> GetPosts(int count, int skip = 0);

        Task<IEnumerable<Post>> GetPostsByCategory(string category);

        Task<Post> GetPostBySlug(string slug);

        Task<Post> GetPostById(string id);

        Task<IEnumerable<string>> GetCategories();

        Task SavePost(Post post);

        Task DeletePost(Post post);

        Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null);
        Task SaveComment(Comment comment);
    }

    public abstract class InMemoryBlogServiceBase : IBlogService
    {
        public InMemoryBlogServiceBase(IHttpContextAccessor contextAccessor)
        {
            ContextAccessor = contextAccessor;
        }

        protected List<Post> Cache { get; set; }
        protected IHttpContextAccessor ContextAccessor { get; }

        public virtual Task<IEnumerable<Post>> GetPosts(int count, int skip = 0)
        {
            bool isAdmin = IsAdmin();

            var posts = Cache
                .Where(p => p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin))
                .Skip(skip)
                .Take(count);

            return Task.FromResult(posts);
        }

        public virtual Task<IEnumerable<Post>> GetPostsByCategory(string category)
        {
            bool isAdmin = IsAdmin();

            var posts = Cache.Where(x=>x.PubDate <= DateTime.UtcNow && (x.IsPublished || isAdmin) && x.PostCategories.Any(c=>c.CategoryName.ToLower() == category.ToLower()));

            return Task.FromResult(posts);

        }

        public virtual Task<Post> GetPostBySlug(string slug)
        {
            var post = Cache.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            bool isAdmin = IsAdmin();

            if (post != null && post.PubDate <= DateTime.UtcNow && (post.IsPublished || isAdmin))
            {
                return Task.FromResult(post);
            }

            return Task.FromResult<Post>(null);
        }

        public virtual Task<Post> GetPostById(string id)
        {
            var post = Cache.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            bool isAdmin = IsAdmin();

            if (post != null && post.PubDate <= DateTime.UtcNow && (post.IsPublished || isAdmin))
            {
                return Task.FromResult(post);
            }

            return Task.FromResult<Post>(null);
        }

        public virtual Task<IEnumerable<string>> GetCategories()
        {
            bool isAdmin = IsAdmin();

            var categories = Cache
                .Where(p => p.IsPublished || isAdmin)
                .SelectMany(post => post.PostCategories)
                .Select(cat => cat.CategoryName.ToLowerInvariant())
                .Distinct();

            return Task.FromResult(categories);
        }

        public abstract Task SavePost(Post post);

        public abstract Task DeletePost(Post post);

        public abstract Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null);

        protected void SortCache()
        {
            Cache.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
        }

        protected bool IsAdmin()
        {
            return ContextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;
        }

        public Task SaveComment(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
