using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Miniblog.Core.Models;

namespace Miniblog.Core.Services
{
    public class DbUserService : IUserServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public DbUserService( UserManager<User> userManager, SignInManager<User> signInManager){
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<bool> ValidateUser(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if(user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}