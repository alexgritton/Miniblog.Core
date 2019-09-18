using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Miniblog.Core.Models;

namespace Miniblog.Core.Services
{
    public class DbBlogService : IBlogService
    {
        public async Task<IEnumerable<Post>> GetPosts(int count, int skip = 0){
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Post>> GetPostsByCategory(string category){
            throw new NotImplementedException();
        }

        public async Task<Post> GetPostBySlug(string slug){
            throw new NotImplementedException();
        }

        public async Task<Post> GetPostById(string id){
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetCategories(){
            throw new NotImplementedException();
        }

        public async Task SavePost(Post post){
            throw new NotImplementedException();
        }

        public async Task DeletePost(Post post){
            throw new NotImplementedException();
        }

        public async Task<string> SaveFile(byte[] bytes, string fileName, string suffix = null){
            throw new NotImplementedException();
        }
    }
}