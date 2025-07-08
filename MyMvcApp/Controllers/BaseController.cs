using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Common;

namespace MyMvcApp.Controllers
{
    /// <summary>
    /// 認証機能を提供するベースコントローラー
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// ログインが必要かどうかを示すフラグ
        /// </summary>
        protected virtual bool RequireAuthentication => true;

        /// <summary>
        /// 認証が不要なアクション名の配列
        /// </summary>
        protected virtual string[] AuthenticationExemptActions => new string[0];

        /// <summary>
        /// アクション実行前の認証チェック
        /// </summary>
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (RequireAuthentication)
            {
                CheckAuthentication(context);
            }
        }

        /// <summary>
        /// 指定されたアクションが認証免除かどうかをチェック
        /// </summary>
        /// <param name="actionName">アクション名</param>
        /// <returns>認証免除の場合true</returns>
        protected bool IsAuthenticationExempt(string actionName)
        {
            return AuthenticationExemptActions.Contains(actionName, System.StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 認証状態をチェックし、未ログインの場合はログイン画面にリダイレクト
        /// </summary>
        protected void CheckAuthentication(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var currentUserId = MyLogger.GetCurrentUserId();

            // セッションIDのみの場合は未ログインとみなす
            if (string.IsNullOrEmpty(currentUserId) || currentUserId.StartsWith("Session:"))
            {
                MyLogger.Instance.Warning("未ログインユーザーがアクセスを試行しました", "BaseController");

                // Ajaxリクエストの場合は403エラーを返す
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new JsonResult(new
                    {
                        success = false,
                        message = "ログインが必要です。未ログインであるためにデータ検索・更新・削除ができません。",
                        requireLogin = true
                    })
                    {
                        StatusCode = 403
                    };
                }
                else
                {
                    // 通常のリクエストの場合はログイン画面にリダイレクト
                    context.Result = new RedirectToActionResult("Index", "Login", null);
                }
            }
        }

        /// <summary>
        /// 現在のユーザーIDを取得
        /// </summary>
        /// <returns>ユーザーID（未ログインの場合はnull）</returns>
        protected string GetCurrentUserId()
        {
            var currentUserId = MyLogger.GetCurrentUserId();
            return string.IsNullOrEmpty(currentUserId) || currentUserId.StartsWith("Session:") ? null : currentUserId;
        }

        /// <summary>
        /// ログイン済みかどうかをチェック
        /// </summary>
        /// <returns>ログイン済みの場合true</returns>
        protected bool IsLoggedIn()
        {
            var currentUserId = GetCurrentUserId();
            return !string.IsNullOrEmpty(currentUserId);
        }
    }
}
