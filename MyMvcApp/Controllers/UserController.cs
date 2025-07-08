using Microsoft.AspNetCore.Mvc;
using MyMvcApp.DAL.Models; // User
using MyMvcApp.Services; // UserService
using System.Threading.Tasks;
using System;
using MyMvcApp.Common;

namespace MyMvcApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
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
            MyLogger.Instance.MethodStart("GetUsers", "", "UserController");

            try
            {
                var users = await _userService.GetAllUsersAsync();
                MyLogger.Instance.Info($"ユーザー一覧を取得しました: {((System.Collections.IEnumerable)users).Cast<object>().Count()}件", "UserController");
                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("ユーザー一覧の取得に失敗しました", ex, "UserController");
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                MyLogger.Instance.MethodEnd("GetUsers", "", "UserController");
            }
        }

        // REST API: 特定のユーザーをJSONで返す
        [HttpGet]
        [Route("api/users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            MyLogger.Instance.MethodStart("GetUser", $"id={id}", "UserController");

            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                MyLogger.Instance.Info($"ユーザー情報を取得しました: ID={id}", "UserController");
                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error($"ユーザー情報の取得に失敗しました: ID={id}", ex, "UserController");
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                MyLogger.Instance.MethodEnd("GetUser", $"id={id}", "UserController");
            }
        }

        // REST API: 新しいユーザーを作成
        [HttpPost]
        [Route("api/users")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            MyLogger.Instance.MethodStart("CreateUser", $"userName={user?.UserName}, email={user?.Email}", "UserController");

            try
            {
                if (!ModelState.IsValid)
                {
                    MyLogger.Instance.Warning("ユーザー作成時にバリデーションエラーが発生しました", "UserController");
                    return Json(new { success = false, message = "入力データが無効です" });
                }

                var createdUser = await _userService.CreateUserAsync(user);
                MyLogger.Instance.Info($"新しいユーザーが作成されました: {user.UserName}", "UserController");
                return Json(new { success = true, data = createdUser, message = "ユーザーが正常に作成されました" });
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("ユーザー作成に失敗しました", ex, "UserController");
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                MyLogger.Instance.MethodEnd("CreateUser", $"userName={user?.UserName}", "UserController");
            }
        }

        // REST API: ユーザーを削除
        [HttpDelete]
        [Route("api/users/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                return Json(new { success = true, message = "ユーザーが正常に削除されました" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
                try
                {
                    await _userService.CreateUserAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(user);
        }

        // POST: /User/Delete/5 (通常のMVCフォーム送信)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
            }
            catch (Exception ex)
            {
                // エラーハンドリング（必要に応じてログ出力など）
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
