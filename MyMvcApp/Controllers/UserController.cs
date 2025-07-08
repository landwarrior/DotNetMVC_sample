using Microsoft.AspNetCore.Mvc;
using MyMvcApp.DAL.Models; // User
using MyMvcApp.Services; // UserService
using System.Threading.Tasks;
using System;
using System.Linq;
using MyMvcApp.Common;

namespace MyMvcApp.Controllers
{
    public class UserController : BaseController
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

        // GET: /User/Create (新規ユーザー登録画面)
        [HttpGet]
        public IActionResult Create()
        {
            // ユーザー登録画面は認証不要（新規ユーザー登録のため）
            return View();
        }

        // POST: /User/Create (新規ユーザー登録処理)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,PasswordHash")] User user)
        {
            MyLogger.Instance.MethodStart("Create", $"userName={user?.UserName}, email={user?.Email}", "UserController");

            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.CreateUserAsync(user);
                    MyLogger.Instance.Info($"新しいユーザーが作成されました: {user.UserName}", "UserController");

                    // 成功メッセージをTempDataに保存
                    TempData["SuccessMessage"] = $"ユーザー「{user.UserName}」が正常に登録されました。";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    MyLogger.Instance.Warning("ユーザー作成時にバリデーションエラーが発生しました", "UserController");
                    // バリデーションエラーメッセージをTempDataに保存
                    var errorMessages = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["ErrorMessage"] = $"バリデーションエラー: {errorMessages}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException ex)
            {
                MyLogger.Instance.Error("ユーザー作成に失敗しました", ex, "UserController");
                // 具体的なエラーメッセージをTempDataに保存
                TempData["ErrorMessage"] = $"ユーザー登録に失敗しました: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("ユーザー作成に失敗しました", ex, "UserController");
                TempData["ErrorMessage"] = $"ユーザー登録に失敗しました: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            finally
            {
                MyLogger.Instance.MethodEnd("Create", $"userName={user?.UserName}", "UserController");
            }
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

        // POST: /User/Delete/5 (通常のMVCフォーム送信)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            MyLogger.Instance.MethodStart("Delete", $"id={id}", "UserController");

            try
            {
                await _userService.DeleteUserAsync(id);
                MyLogger.Instance.Info($"ユーザーが削除されました: ID={id}", "UserController");

                // 成功メッセージをTempDataに保存
                TempData["SuccessMessage"] = "ユーザーが正常に削除されました。";
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error($"ユーザー削除に失敗しました: ID={id}", ex, "UserController");
                TempData["ErrorMessage"] = $"ユーザーの削除に失敗しました: {ex.Message}";
            }
            finally
            {
                MyLogger.Instance.MethodEnd("Delete", $"id={id}", "UserController");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
