using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Services;
using MyMvcApp.Common;
using System.Threading.Tasks;
using System;

namespace MyMvcApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// ログイン画面を表示
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            MyLogger.Instance.MethodStart("Index", "", "LoginController");

            // 既にログイン済みの場合はホーム画面にリダイレクト
            var currentUserId = MyLogger.GetCurrentUserId();
            if (!string.IsNullOrEmpty(currentUserId) && !currentUserId.StartsWith("Session:"))
            {
                MyLogger.Instance.Info($"既にログイン済みのためホーム画面にリダイレクト: {currentUserId}", "LoginController");
                return RedirectToAction("Index", "Home");
            }

            MyLogger.Instance.MethodEnd("Index", "", "LoginController");
            return View();
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string userName, string password)
        {
            MyLogger.Instance.MethodStart("Index", $"userName={userName}", "LoginController");

            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    MyLogger.Instance.Warning("ログイン失敗: ユーザー名またはパスワードが空です", "LoginController");
                    ModelState.AddModelError("", "ユーザー名とパスワードを入力してください");
                    return View();
                }

                // 簡易的な認証処理
                // ユーザー名とパスワードが一致すればログイン成功
                if (userName == password)
                {
                    // セッションにユーザーIDを保存（ユーザー名をIDとして使用）
                    MyLogger.SetUserIdToSession(userName);
                    MyLogger.Instance.Info($"ユーザーログイン成功: {userName}", "LoginController");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    MyLogger.Instance.Warning($"ログイン失敗: ユーザー名とパスワードが一致しません - {userName}", "LoginController");
                    ModelState.AddModelError("", "ユーザー名またはパスワードが正しくありません");
                    return View();
                }
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("ログイン処理でエラーが発生しました", ex, "LoginController");
                ModelState.AddModelError("", "ログイン処理でエラーが発生しました");
                return View();
            }
            finally
            {
                MyLogger.Instance.MethodEnd("Index", $"userName={userName}", "LoginController");
            }
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            MyLogger.Instance.MethodStart("Logout", "", "LoginController");

            try
            {
                var currentUserId = MyLogger.GetCurrentUserId();

                // セッションからユーザーIDを削除
                MyLogger.ClearUserIdFromSession();
                MyLogger.Instance.Info($"ユーザーログアウト完了: {currentUserId}", "LoginController");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("ログアウト処理でエラーが発生しました", ex, "LoginController");
                return RedirectToAction("Index", "Home");
            }
            finally
            {
                MyLogger.Instance.MethodEnd("Logout", "", "LoginController");
            }
        }
    }
}
