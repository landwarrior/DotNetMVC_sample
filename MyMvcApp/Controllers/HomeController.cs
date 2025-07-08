using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;
using MyMvcApp.Common;

namespace MyMvcApp.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 認証が必要に設定
    /// </summary>
    protected override bool RequireAuthentication => true;

    public IActionResult Index()
    {
        MyLogger.Instance.MethodStart("Index", "", "HomeController");

        try
        {
            // 現在のユーザーIDを取得（セッション情報から自動取得）
            var currentUserId = MyLogger.GetCurrentUserId();

            if (!string.IsNullOrEmpty(currentUserId) && !currentUserId.StartsWith("Session:"))
            {
                MyLogger.Instance.Info($"ログイン済みユーザーがアクセスしました: {currentUserId}", "HomeController");
                ViewBag.CurrentUserId = currentUserId;
                ViewBag.IsLoggedIn = true;
            }
            else
            {
                MyLogger.Instance.Info("未ログインユーザーがアクセスしました", "HomeController");
                ViewBag.IsLoggedIn = false;
            }

            MyLogger.Instance.MethodEnd("Index", "", "HomeController");
            return View();
        }
        catch (Exception ex)
        {
            MyLogger.Instance.Error("ホームページの表示でエラーが発生しました", ex, "HomeController");
            return View();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
