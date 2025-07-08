using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.DAL;
using MyMvcApp.DAL.Models;

namespace MyMvcApp.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .AsNoTracking()
                    .Select(u => new
                    {
                        id = u.Id,
                        userName = u.UserName,
                        email = u.Email,
                        createdAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToListAsync();

                return users;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("ユーザー一覧の取得に失敗しました");
            }
        }

        public async Task<object> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == id)
                    .Select(u => new
                    {
                        id = u.Id,
                        userName = u.UserName,
                        email = u.Email,
                        createdAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new InvalidOperationException("指定されたユーザーが見つかりません");
                }

                return user;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("ユーザー情報の取得に失敗しました");
            }
        }

        public async Task<object> CreateUserAsync(User user)
        {
            try
            {
                // ユーザー名の重複チェック
                if (await IsUserNameExistsAsync(user.UserName))
                {
                    throw new InvalidOperationException("このユーザー名は既に使用されています");
                }

                // メールアドレスの重複チェック
                if (await IsEmailExistsAsync(user.Email))
                {
                    throw new InvalidOperationException("このメールアドレスは既に使用されています");
                }

                user.CreatedAt = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();

                var createdUser = new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    createdAt = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };

                return createdUser;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("ユーザーの作成に失敗しました");
            }
        }

        public async Task<object> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new InvalidOperationException("指定されたユーザーが見つかりません");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new { message = "ユーザーが正常に削除されました" };
            }
            catch (Exception)
            {
                throw new InvalidOperationException("ユーザーの削除に失敗しました");
            }
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
