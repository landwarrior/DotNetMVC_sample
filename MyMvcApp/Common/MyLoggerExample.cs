using System;
using System.Threading.Tasks;

namespace MyMvcApp.Common
{
    /// <summary>
    /// MyLoggerクラスの使用例を示すサンプルクラス
    /// </summary>
    public class MyLoggerExample
    {
        /// <summary>
        /// 基本的なログ出力の例
        /// </summary>
        public void BasicLoggingExample()
        {
            // 情報レベルのログ
            MyLogger.Instance.Info("アプリケーションが開始されました", "Application");

            // 警告レベルのログ
            MyLogger.Instance.Warning("設定ファイルが見つかりません。デフォルト設定を使用します。", "Configuration");

            // エラーレベルのログ
            MyLogger.Instance.Error("データベース接続に失敗しました", "Database");

            // 例外情報を含むエラーログ
            try
            {
                throw new InvalidOperationException("テスト例外");
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("処理中に例外が発生しました", ex, "Exception");
            }

            // デバッグレベルのログ（デバッグビルドでのみ出力）
            MyLogger.Instance.Debug("デバッグ情報: 変数xの値は10です", "Debug");
        }

        /// <summary>
        /// メソッドの開始・終了ログの例
        /// </summary>
        public async Task<string> MethodLoggingExample(string input)
        {
            MyLogger.Instance.MethodStart("MethodLoggingExample", $"input={input}", "Example");

            try
            {
                // 何らかの処理
                await Task.Delay(100);
                var result = $"処理結果: {input.ToUpper()}";

                MyLogger.Instance.MethodEnd("MethodLoggingExample", $"result={result}", "Example");
                return result;
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("MethodLoggingExampleでエラーが発生しました", ex, "Example");
                throw;
            }
        }

                /// <summary>
        /// パフォーマンス計測の例（参考実装）
        /// </summary>
        public async Task PerformanceLoggingExample()
        {
            // Stopwatchは高精度な時間計測を行うためのクラスです
            // StartNew()で計測開始、Stop()で計測終了、ElapsedMillisecondsで経過時間（ミリ秒）を取得
            // var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // 何らかの重い処理
                await Task.Delay(500);

                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                // MyLogger.Instance.Performance("重い処理", stopwatch.ElapsedMilliseconds, "Performance");
            }
            catch (Exception ex)
            {
                // パフォーマンス計測の例（参考実装）
                // stopwatch.Stop();
                MyLogger.Instance.Error("パフォーマンス計測中にエラーが発生しました", ex, "Performance");
                throw;
            }
        }

        /// <summary>
        /// カテゴリ別ログ出力の例
        /// </summary>
        public void CategoryLoggingExample()
        {
            // 認証関連のログ
            MyLogger.Instance.Info("ユーザーログイン試行", "Authentication");
            MyLogger.Instance.Warning("パスワードが間違っています", "Authentication");

            // データベース関連のログ
            MyLogger.Instance.Info("データベース接続確立", "Database");
            MyLogger.Instance.Error("クエリ実行に失敗しました", "Database");

            // ファイル操作関連のログ
            MyLogger.Instance.Info("ファイル読み込み開始", "FileOperation");
            MyLogger.Instance.Info("ファイル読み込み完了", "FileOperation");

            // 外部API関連のログ
            MyLogger.Instance.Info("外部API呼び出し開始", "ExternalAPI");
            MyLogger.Instance.Warning("外部API応答時間が長いです", "ExternalAPI");
        }

        /// <summary>
        /// 条件付きログ出力の例
        /// </summary>
        public void ConditionalLoggingExample(bool isDebugMode)
        {
            MyLogger.Instance.Info("アプリケーション処理開始", "Conditional");

            if (isDebugMode)
            {
                MyLogger.Instance.Debug("デバッグモードで実行中", "Conditional");
                MyLogger.Instance.Debug("詳細な処理情報を出力します", "Conditional");
            }

            // 重要な処理
            MyLogger.Instance.Info("重要な処理を実行中", "Conditional");

            if (isDebugMode)
            {
                MyLogger.Instance.Debug("処理の詳細情報", "Conditional");
            }

            MyLogger.Instance.Info("アプリケーション処理完了", "Conditional");
        }

                /// <summary>
        /// 自動的な呼び出し元情報の取得例
        /// </summary>
        public void CallerInfoExample()
        {
            // 呼び出し元のファイル名、メソッド名、行番号が自動的に記録されます
            MyLogger.Instance.Info("このメッセージには呼び出し元情報が自動的に含まれます");

            // カテゴリを指定した場合
            MyLogger.Instance.Warning("警告メッセージ", "TestCategory");

            // エラーの場合も同様
            MyLogger.Instance.Error("エラーメッセージ", "TestCategory");
        }

                /// <summary>
        /// ユーザーIDとタイムスタンプの使用例（将来的なログイン機能実装用）
        /// </summary>
        public void UserIdAndTimestampExample()
        {
            // 現在のユーザーIDを取得
            string currentUserId = MyLogger.GetCurrentUserId();
            MyLogger.Instance.Info($"現在のユーザーID: {currentUserId}", "User");

            // ユーザーIDを手動で設定（将来的なログイン機能で自動設定予定）
            MyLogger.SetUserId("USER123");
            MyLogger.Instance.Info("カスタムユーザーIDでログ出力", "User");

            // 元のユーザーIDに戻す
            MyLogger.SetUserId(currentUserId);
            MyLogger.Instance.Info("元のユーザーIDに戻しました", "User");

            // タイムスタンプ付きのログ出力例
            MyLogger.Instance.Info("タイムスタンプが自動的に付加されます", "Timestamp");
            MyLogger.Instance.Warning("警告メッセージも同様です", "Timestamp");
            MyLogger.Instance.Error("エラーメッセージも同様です", "Timestamp");
        }
    }
}
