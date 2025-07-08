using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MyMvcApp.Common
{
    /// <summary>
    /// 汎用的なログ出力クラス
    /// System.Diagnostics.Traceを使用してログを出力します
    /// セッション情報から自動的にユーザーIDを取得します
    /// </summary>
    public class MyLogger
    {
        private static readonly MyLogger _instance = new MyLogger();

        // シングルトンパターン
        private MyLogger() { }

        public static MyLogger Instance => _instance;

        /// <summary>
        /// 現在のユーザーID（将来的なログイン機能実装用）
        /// </summary>
        private static string _currentUserId = string.Empty;

        /// <summary>
        /// HttpContextアクセサー（DIコンテナから取得）
        /// </summary>
        private static IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// HttpContextアクセサーを設定（Program.csで設定）
        /// </summary>
        /// <param name="httpContextAccessor">HttpContextアクセサー</param>
        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// セッションからユーザーIDを自動取得
        /// </summary>
        /// <returns>ユーザーID（取得できない場合は空文字）</returns>
        private static string GetUserIdFromSession()
        {
            try
            {
                if (_httpContextAccessor?.HttpContext == null)
                {
                    return string.Empty;
                }

                var httpContext = _httpContextAccessor.HttpContext;

                // セッションが利用可能かチェック
                if (!httpContext.Session.IsAvailable)
                {
                    return string.Empty;
                }

                // セッションからユーザーIDを取得
                var userId = httpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    return userId;
                }

                // セッションIDをフォールバックとして使用
                var sessionId = httpContext.Session.Id;
                if (!string.IsNullOrEmpty(sessionId))
                {
                    return $"Session:{sessionId}";
                }

                return string.Empty;
            }
            catch (Exception)
            {
                // セッション取得でエラーが発生した場合は空文字を返す
                return string.Empty;
            }
        }

        /// <summary>
        /// 現在のユーザーIDを取得（セッション情報を優先）
        /// </summary>
        public static string GetCurrentUserId()
        {
            // まずセッションから取得を試行
            var sessionUserId = GetUserIdFromSession();
            if (!string.IsNullOrEmpty(sessionUserId))
            {
                return sessionUserId;
            }

            // セッションから取得できない場合は静的フィールドから取得
            return _currentUserId;
        }

        /// <summary>
        /// ユーザーIDを設定（将来的なログイン機能実装用）
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public static void SetUserId(string userId)
        {
            _currentUserId = userId;
        }

        /// <summary>
        /// セッションにユーザーIDを保存
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public static void SetUserIdToSession(string userId)
        {
            try
            {
                if (_httpContextAccessor?.HttpContext?.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("UserId", userId);
                }
            }
            catch (Exception)
            {
                // セッション保存でエラーが発生した場合は無視
            }
        }

        /// <summary>
        /// セッションからユーザーIDを削除
        /// </summary>
        public static void ClearUserIdFromSession()
        {
            try
            {
                if (_httpContextAccessor?.HttpContext?.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.Remove("UserId");
                }
            }
            catch (Exception)
            {
                // セッション削除でエラーが発生した場合は無視
            }
        }

        /// <summary>
        /// 情報レベルのログを出力
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        public void Info(string message, string category = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = FormatLogMessage("INFO", message, category, filePath, memberName, lineNumber);
            Trace.TraceInformation(logMessage);
        }

        /// <summary>
        /// 警告レベルのログを出力
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        public void Warning(string message, string category = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = FormatLogMessage("WARNING", message, category, filePath, memberName, lineNumber);
            Trace.TraceWarning(logMessage);
        }

        /// <summary>
        /// エラーレベルのログを出力
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        public void Error(string message, string category = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = FormatLogMessage("ERROR", message, category, filePath, memberName, lineNumber);
            Trace.TraceError(logMessage);
        }

        /// <summary>
        /// 例外情報を含むエラーログを出力
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="exception">例外オブジェクト</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        public void Error(string message, Exception exception, string category = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = FormatLogMessage("ERROR", message, category, filePath, memberName, lineNumber);
            Trace.TraceError($"{logMessage} - Exception: {exception.Message}");
            Trace.TraceError($"[ERROR][{category}] Stack Trace: {exception.StackTrace}");
        }

        /// <summary>
        /// デバッグレベルのログを出力（デバッグビルドでのみ出力）
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        [Conditional("DEBUG")]
        public void Debug(string message, string category = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = FormatLogMessage("DEBUG", message, category, filePath, memberName, lineNumber);
            Trace.WriteLine(logMessage);
        }

        /// <summary>
        /// メソッドの開始をログ出力
        /// </summary>
        /// <param name="methodName">メソッド名</param>
        /// <param name="parameters">パラメータ情報（オプション）</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        public void MethodStart(string methodName, string parameters = "", string category = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = $"Method Start: {methodName}";
            if (!string.IsNullOrEmpty(parameters))
            {
                logMessage += $" - Parameters: {parameters}";
            }
            Info(logMessage, category, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// メソッドの終了をログ出力
        /// </summary>
        /// <param name="methodName">メソッド名</param>
        /// <param name="result">戻り値情報（オプション）</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        public void MethodEnd(string methodName, string result = "", string category = "",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = $"Method End: {methodName}";
            if (!string.IsNullOrEmpty(result))
            {
                logMessage += $" - Result: {result}";
            }
            Info(logMessage, category, filePath, memberName, lineNumber);
        }

        /// <summary>
        /// パフォーマンス計測用のログ出力
        /// 使用例：
        /// var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        /// // 何らかの処理
        /// stopwatch.Stop();
        /// MyLogger.Instance.Performance("処理名", stopwatch.ElapsedMilliseconds, "カテゴリ");
        /// </summary>
        /// <param name="operation">操作名</param>
        /// <param name="elapsedMilliseconds">経過時間（ミリ秒）</param>
        /// <param name="category">ログカテゴリ（オプション）</param>
        /// <param name="filePath">呼び出し元ファイルパス（自動取得）</param>
        /// <param name="memberName">呼び出し元メソッド名（自動取得）</param>
        /// <param name="lineNumber">呼び出し元行番号（自動取得）</param>
        public void Performance(string operation, long elapsedMilliseconds, string category = "Performance",
            [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            var message = $"{operation} - {elapsedMilliseconds}ms";
            var logMessage = FormatLogMessage("PERFORMANCE", message, category, filePath, memberName, lineNumber);
            Trace.TraceInformation(logMessage);
        }

        /// <summary>
        /// ログメッセージをフォーマット
        /// </summary>
        private string FormatLogMessage(string level, string message, string category, string filePath, string memberName, int lineNumber)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            // 現在のユーザーIDを取得（セッション情報を優先）
            var currentUserId = GetCurrentUserId();
            var userInfo = !string.IsNullOrEmpty(currentUserId) ? $"[UID:{currentUserId}]" : "";
            var cat = !string.IsNullOrEmpty(category) ? $"({category})" : "";

            return $"{timestamp} [{level}]{userInfo}{cat} {message} [in {fileName}::{memberName}:{lineNumber}]";
        }
    }
}
