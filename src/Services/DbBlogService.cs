using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericBlog.Data;
using Microsoft.EntityFrameworkCore;
using Miniblog.Core.Models;

namespace Miniblog.Core.Services
{
    public class DbBlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        public DbBlogService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Post>> GetPosts(int count, int skip = 0)
        {
            IQueryable<Post> posts = _context.Post.AsNoTracking();
            posts = posts.Skip(skip).Take(count);

            return await posts.Include(x=>x.Comments).Include(x=>x.PostCategories).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByCategory(string category)
        {
            IQueryable<PostCategory> postCats = _context.PostCategory.AsNoTracking();
            postCats = postCats.Where(x => x.CategoryName == category).Include(x => x.Post);
            return await postCats.Select(x => x.Post).Include(x=>x.Comments).Include(x=>x.PostCategories).ToListAsync();
        }

        public async Task<Post> GetPostBySlug(string slug)
        {
            IQueryable<Post> posts = _context.Post.AsNoTracking();
            return await posts.Include(x=>x.Comments).Include(x=>x.PostCategories).FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task<Post> GetPostById(string id)
        {
            IQueryable<Post> posts = _context.Post.AsNoTracking();
            return await posts.Include(x=>x.Comments).Include(x=>x.PostCategories).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<string>> GetCategories()
        {
            IQueryable<Category> query = _context.Category.AsNoTracking();
            return await query.Select(x => x.Name).ToListAsync();
        }

        public async Task SavePost(Post post)
        {
            var postCats = new List<PostCategory>(post.PostCategories);
            post.PostCategories = null;
            post.LastModified = DateTime.UtcNow;
            if (post.IsPublished && post.PubDate == DateTime.MinValue) post.PubDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(post.Id))
            {
                post.Id = Guid.NewGuid().ToString();
                await _context.AddAsync(post);
            }
            else
            {
                //get past postcats and remove any not in postcats passed in with post.
                _context.Update(post);
            }
            await _context.SaveChangesAsync();
            
            var oldPostCats = await _context.PostCategory.AsNoTracking().Where(x=>x.PostId == post.Id).ToListAsync();
            foreach(var postCat in postCats){
                if(oldPostCats.Any(x=>x.CategoryName == postCat.CategoryName)) continue;
                await SavePostCategory(postCat);
            }
            foreach(var oldPostCat in oldPostCats){
                if(postCats.Any(x=>x.CategoryName == oldPostCat.CategoryName)) continue;
                await DeletePostCategory(oldPostCat);
            }

        }

        public async Task DeletePost(Post post)
        {
            if (!string.IsNullOrEmpty(post.Id))
            {
                _context.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveComment(Comment comment){
            if(string.IsNullOrEmpty(comment.Id)){
                comment.Id = Guid.NewGuid().ToString();
                comment.PubDate = DateTime.UtcNow;
                await _context.AddAsync(comment);
            }
            else{
                _context.Update(comment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(Comment comment){
            if(!string.IsNullOrEmpty(comment.Id)){
                _context.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveCategory(Category category){
            category.Name = category.Name.ToLower();
            var existing = await _context.Category.AsNoTracking().FirstOrDefaultAsync(x=>x.Name == category.Name);
            if(existing != null) return;
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task SavePostCategory(PostCategory category){
            if(category.Category == null && string.IsNullOrEmpty(category.CategoryName)) return;
            if(category.Post == null && string.IsNullOrEmpty(category.PostId)) return;
            category.CategoryName = category.Category == null ? category.CategoryName.ToLower() : category.Category.Name.ToLower();
            category.PostId = category.Post == null ? category.PostId : category.Post.Id;
            await SaveCategory(category.Category);   
            category.Category = null;
            category.Post = null;         
            var existing = await _context.PostCategory.AsNoTracking().FirstOrDefaultAsync(x=>x.CategoryName == category.CategoryName && x.PostId == category.PostId);
            if(existing!= null) return;
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostCategory(PostCategory category){
            if(category.Category == null && string.IsNullOrEmpty(category.CategoryName)) return;
            if(category.Post == null && string.IsNullOrEmpty(category.PostId)) return;
            _context.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null)
        {
            throw new NotImplementedException();
        }
    }
}