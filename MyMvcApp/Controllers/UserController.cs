using Microsoft.AspNetCore.Mvc;
using MyMvcApp.DAL; // DbContext
using MyMvcApp.DAL.Models; // User
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MyMvcApp.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [Route("user")]
        public async Task<IActionResult> Index()
        {
            // 通常のMVCビューを返す（JavaScriptでAjax呼び出しを行うページ）
            return View();
        }

        // REST API: ユーザー一覧をJSONで返す
        [HttpGet]
        [Route("api/users")]
        public async Task<IActionResult> GetUsers()
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

                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ユーザー一覧の取得に失敗しました" });
            }
        }

        // REST API: 特定のユーザーをJSONで返す
        [HttpGet]
        [Route("api/users/{id}")]
        public async Task<IActionResult> GetUser(int id)
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
                    return Json(new { success = false, message = "指定されたユーザーが見つかりません" });
                }

                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ユーザー情報の取得に失敗しました" });
            }
        }

        // REST API: 新しいユーザーを作成
        [HttpPost]
        [Route("api/users")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "入力データが無効です" });
                }

                // ユーザー名の重複チェック
                if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
                {
                    return Json(new { success = false, message = "このユーザー名は既に使用されています" });
                }

                // メールアドレスの重複チェック
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return Json(new { success = false, message = "このメールアドレスは既に使用されています" });
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

                return Json(new { success = true, data = createdUser, message = "ユーザーが正常に作成されました" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ユーザーの作成に失敗しました" });
            }
        }

        // REST API: ユーザーを削除
        [HttpDelete]
        [Route("api/users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "指定されたユーザーが見つかりません" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "ユーザーが正常に削除されました" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ユーザーの削除に失敗しました" });
            }
        }

        // GET: /User/Create (通常のMVCビュー)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /User/Create (通常のMVCフォーム送信)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,PasswordHash")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // POST: /User/Delete/5 (通常のMVCフォーム送信)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
