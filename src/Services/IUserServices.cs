﻿using System.Threading.Tasks;

namespace Miniblog.Core.Services
{
    public interface IUserServices
    {
        Task<bool> ValidateUser(string username, string password);
        Task CreateUser(string username, string password);
    }
}
