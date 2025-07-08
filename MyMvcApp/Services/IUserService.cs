using System.Collections.Generic;
using System.Threading.Tasks;
using MyMvcApp.DAL.Models;

namespace MyMvcApp.Services
{
    public interface IUserService
    {
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<object> GetUserByIdAsync(int id);
        Task<object> CreateUserAsync(User user);
        Task<object> DeleteUserAsync(int id);
        Task<bool> IsUserNameExistsAsync(string userName);
        Task<bool> IsEmailExistsAsync(string email);
        Task<User> GetUserByUserNameAsync(string userName);
    }
}
