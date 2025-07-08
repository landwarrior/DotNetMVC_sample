using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.DAL;
using MyMvcApp.DAL.Models;
using MyMvcApp.Common;

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
            MyLogger.Instance.MethodStart("GetAllUsersAsync", "", "UserService");

            // パフォーマンス計測の例（参考実装）
            // Stopwatchは高精度な時間計測を行うためのクラスです
            // StartNew()で計測開始、Stop()で計測終了、ElapsedMillisecondsで経過時間（ミリ秒）を取得
            // var stopwatch = System.Diagnostics.Stopwatch.StartNew();

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

                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                // MyLogger.Instance.Performance("GetAllUsersAsync", stopwatch.ElapsedMilliseconds, "UserService");

                MyLogger.Instance.MethodEnd("GetAllUsersAsync", $"Retrieved {users.Count()} users", "UserService");

                return users;
            }
            catch (Exception ex)
            {
                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                MyLogger.Instance.Error("ユーザー一覧の取得に失敗しました", ex, "UserService");
                throw new InvalidOperationException("ユーザー一覧の取得に失敗しました");
            }
        }

                public async Task<object> GetUserByIdAsync(int id)
        {
            MyLogger.Instance.MethodStart("GetUserByIdAsync", $"id={id}", "UserService");

            // パフォーマンス計測の例（参考実装）
            // var stopwatch = System.Diagnostics.Stopwatch.StartNew();

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
                    MyLogger.Instance.Warning($"指定されたユーザーが見つかりません: id={id}", "UserService");
                    throw new InvalidOperationException("指定されたユーザーが見つかりません");
                }

                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                // MyLogger.Instance.Performance("GetUserByIdAsync", stopwatch.ElapsedMilliseconds, "UserService");

                MyLogger.Instance.MethodEnd("GetUserByIdAsync", $"User found: {user.userName}", "UserService");

                return user;
            }
            catch (Exception ex)
            {
                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                MyLogger.Instance.Error("ユーザー情報の取得に失敗しました", ex, "UserService");
                throw new InvalidOperationException("ユーザー情報の取得に失敗しました");
            }
        }

                public async Task<object> CreateUserAsync(User user)
        {
            MyLogger.Instance.MethodStart("CreateUserAsync", $"userName={user.UserName}, email={user.Email}", "UserService");

            // パフォーマンス計測の例（参考実装）
            // var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // ユーザー名の重複チェック
                if (await IsUserNameExistsAsync(user.UserName))
                {
                    MyLogger.Instance.Warning($"ユーザー名が重複しています: {user.UserName}", "UserService");
                    throw new InvalidOperationException("このユーザー名は既に使用されています");
                }

                // メールアドレスの重複チェック
                if (await IsEmailExistsAsync(user.Email))
                {
                    MyLogger.Instance.Warning($"メールアドレスが重複しています: {user.Email}", "UserService");
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

                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                // MyLogger.Instance.Performance("CreateUserAsync", stopwatch.ElapsedMilliseconds, "UserService");

                MyLogger.Instance.MethodEnd("CreateUserAsync", $"User created with ID: {user.Id}", "UserService");
                MyLogger.Instance.Info($"新しいユーザーが作成されました: {user.UserName} (ID: {user.Id})", "UserService");

                return createdUser;
            }
            catch (Exception ex)
            {
                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                MyLogger.Instance.Error("ユーザーの作成に失敗しました", ex, "UserService");
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
