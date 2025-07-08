# DotNetMVC_sample

ASP.NET MVC と .NET8 で Web アプリのサンプルを作ってみる

## 初期セットアップ

プロジェクト作成は以下のコマンドを実行したと思う。
```console
dotnet new mvc -n DotNetMVC_sample -o MyMvcApp
```
基本的に [ASP.NET Core MVC の概要 | Microsoft Learn](https://learn.microsoft.com/ja-jp/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0&tabs=visual-studio) と同じことをしている。  
Visual Studio の場合は「ASP.NET Core Web アプリ (Model-View-Controller)」を選択した場合と同じ。

VS Code だと複数のプロジェクトをソリューションに入れる方法がよく分からないので残りは Visual Studio でやってくことにする。  
ただし README は Visual Studio のソリューションエクスプローラーに表示されないので VS Code で編集する。

### NuGet パッケージの追加（MyMvcApp.DAL）

ツール → NuGet パッケージ マネージャー → パッケージ マネージャー コンソール を開き、以下を実行
```console
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.Sqlite
```
※ DB は SQLite を使います
※ PowerShell を実行する前に、プロジェクトが MyMvcApp.DAL になっていることを確認する

### 参照設定

MyMvcApp プロジェクトを右クリック → 追加 → プロジェクト参照 → MyMvcApp.DAL を選択して追加

### NuGet パッケージの追加（MyMvcApp）

ツール → NuGet パッケージ マネージャー → パッケージ マネージャー コンソール を開き、以下を実行
```console
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package Microsoft.EntityFrameworkCore.Tools
```
※ PowerShell を実行する前に、プロジェクトが MyMvcApp になっていることを確認する

### いろいろとコードを修正

1. Program.cs で DbContext を登録 → これはソースコードにすでに書いてあるから割愛
2. appsettings.json に接続文字列を追加 → 追加したのは以下
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Data Source=app.db"
      }
    }
    ```

### マイグレーションと DB 作成

パッケージマネージャーコンソールで以下を実行（プロジェクトがどこになっていても関係ない）
```console
Add-Migration InitialCreate -Project MyMvcApp.DAL -StartupProject MyMvcApp
Update-Database -Project MyMvcApp.DAL -StartupProject MyMvcApp
```
モデルクラスを追加したりしてマイグレーションをやり直す場合は、最初のコマンドの `InitialCreate` を適当な名前にしないとマイグレーションを適用できない。  
名称変更して最初のコマンドをしたら、2つ目のコマンドはそのまま実行で問題ない。


## 実行方法( VS Code の場合)

Program.cs を開いて F5 を押すとなんか出てくるので、 C# を選んで ASP.NET MVC だったかで Default Configuration だったかを選べばとりあえず動くはず。  
Visual Studio の場合は考えなくても普通に実行できるはず。

---

ここから下は一度試して辞めたやつで、今後直すつもり

## minify 方法（これはダメっぽい）

1. パッケージのインストール  
    Visual Studio のパッケージマネージャーコンソールまたはターミナルで以下のコマンドを実行します
    ```sh
    dotnet add package BuildBundlerMinifier
    ```
2. bundleconfig.json の作成  
    プロジェクトのルートディレクトリに `bundleconfig.json` ファイルを作成します
    ```json
    [
        {
            "outputFileName": "wwwroot/css/site.min.css",
            "inputFiles": [
                "wwwroot/css/site.css"
            ],
            "minify": {
            "enabled": true
            }
        },
        {
            "outputFileName": "wwwroot/js/site.min.js",
            "inputFiles": [
                "wwwroot/js/site.js"
            ],
            "minify": {
            "enabled": true,
            "renameLocals": true
            }
        }
    ]
    ```
3. ビルド時の自動実行設定  
    `csproj` ファイルに以下の設定を追加します
    ```xml
    <Project Sdk="Microsoft.NET.Sdk.Web">
        // ...existing code...
        <ItemGroup>
            <PackageReference Include="BuildBundlerMinifier" Version="3.2.449" />
        </ItemGroup>

        <Target Name="Bundle" BeforeTargets="Build">
            <Exec Command="dotnet bundle" />
        </Target>
        // ...existing code...
    </Project>
    ```
4. レイアウトファイルの更新  
    `_Layout.cshtml` で minify されたファイルを参照するように更新します
    ```xml
    <!DOCTYPE html>
    <html>
    <head>
        // ...existing code...
        <link rel="stylesheet" href="~/css/site.min.css" />
    </head>
    <body>
        // ...existing code...
        <script src="~/js/site.min.js"></script>
    </body>
    </html>
    ```

手動での実行

必要に応じて、以下のコマンドで手動で minify を実行できます
```sh
dotnet bundle
```

この設定により

- CSS と JavaScript ファイルが自動的に minify されます
- ビルド時に自動的に実行されます
- 開発時はソースマップが生成されます
- 本番環境では最適化されたファイルが使用されます

開発環境では、`bundleconfig.json` の設定で `minify.enabled` を `false` にすることで、デバッグをしやすくすることもできます。

## バンドルと縮小

1. パッケージのインストール
    ```sh
    dotnet add package WebOptimizer.Core
    ```
2. Startup.cs または Program.cs での設定
    ```cs
    var builder = WebApplication.CreateBuilder(args);

    // WebOptimizer の追加
    builder.Services.AddWebOptimizer(pipeline =>
    {
        // CSS の最適化
        pipeline.AddCssBundle("/css/bundle.min.css", 
            "css/site.css",
            "Views/Shared/_Layout.cshtml.css");

        // JavaScript の最適化
        pipeline.AddJavaScriptBundle("/js/bundle.min.js",
            "js/site.js");
    });

    // ...existing code...

    var app = builder.Build();

    // WebOptimizer ミドルウェアの使用
    app.UseWebOptimizer();

    // ...existing code...
    ```
3. レイアウトファイルの更新
    ```html
    <!DOCTYPE html>
    <html>
    <head>
        // ...existing code...
        <link rel="stylesheet" href="/css/bundle.min.css" asp-append-version="true" />
    </head>
    <body>
        // ...existing code...
        <script src="/js/bundle.min.js" asp-append-version="true"></script>
    </body>
    </html>
    ```
4. 開発環境での設定（オプション）
    ```json
    {
        // ...existing code...
        "WebOptimizer": {
            "EnableCaching": false,
            "EnableDiskCache": false,
            "EnableMemoryCache": false,
            "EnableTagHelperBundling": false
        }
    }
    ```

WebOptimizer の利点：

- ASP.NET Core に最適化された設計
- キャッシュの自動管理
- タグヘルパーのサポート
- 開発環境とプロダクション環境での自動切り替え
- HTTP/2 のサポート
- ソースマップの自動生成
- CDN のサポート

BuildBundlerMinifier と比較して、より柔軟な設定とパフォーマンス最適化が可能です。

## ログ機能 (MyLogger)

### MyLoggerクラスの概要

`MyLogger`クラスは、`System.Diagnostics.Trace`を使用した汎用的なログ出力クラスです。
タイムスタンプが自動的に付加され、将来的なログイン機能実装時にユーザーIDも含めることができます。

### 主な機能

- **タイムスタンプ**: 各ログに自動的に日時が付加されます
- **ユーザーID**: 将来的なログイン機能実装時にユーザーIDを付加可能です
- **呼び出し元情報**: ファイル名、メソッド名、行番号が自動的に記録されます
- **カテゴリ別分類**: オプションでカテゴリを指定可能です

### 基本的な使用方法

```csharp
using MyMvcApp.Common;

// 情報レベルのログ
MyLogger.Instance.Info("アプリケーションが開始されました");

// 警告レベルのログ
MyLogger.Instance.Warning("設定ファイルが見つかりません");

// エラーレベルのログ
MyLogger.Instance.Error("データベース接続に失敗しました");

// 例外情報を含むエラーログ
try
{
    // 何らかの処理
}
catch (Exception ex)
{
    MyLogger.Instance.Error("処理中に例外が発生しました", ex);
}

// デバッグレベルのログ（デバッグビルドでのみ出力）
MyLogger.Instance.Debug("デバッグ情報");

// カテゴリを指定した場合
MyLogger.Instance.Info("カテゴリ付きログ", "Application");
MyLogger.Instance.Warning("認証エラー", "Authentication");
MyLogger.Instance.Error("データベースエラー", "Database");
```

### ログ出力例

```
2024-01-15 14:30:25.123 [INFO] アプリケーションが開始されました [in Program.cs::Main:25]
2024-01-15 14:30:25.456 [INFO] ユーザー一覧を取得しました: 5件 [in UserController.cs::GetUsers:30]
2024-01-15 14:30:25.789 [INFO][UID:USER123] ログインユーザーによる操作 [in UserController.cs::CreateUser:45]
2024-01-15 14:30:26.012 [WARNING](Authentication) 認証エラー [in AuthController.cs::Login:15]
2024-01-15 14:30:26.345 [ERROR](Database) データベースエラー [in UserService.cs::GetUserById:42]
```

### ユーザーIDの管理（将来的なログイン機能実装用）

```csharp
// 現在のユーザーIDを取得
string userId = MyLogger.GetCurrentUserId();

// ユーザーIDを手動で設定（将来的なログイン機能で自動設定予定）
MyLogger.SetUserId("USER123");

// ユーザーIDが設定されている場合、ログに自動的に含まれる
MyLogger.Instance.Info("ログインユーザーによる操作");

// カテゴリとユーザーIDを組み合わせた場合
MyLogger.Instance.Info("ログインユーザーによる操作", "UserAction");
MyLogger.Instance.Warning("ログインユーザーの警告", "UserAction");
MyLogger.Instance.Error("ログインユーザーのエラー", "UserAction");
```

### メソッドの開始・終了ログ

```csharp
public async Task<string> SomeMethod(string input)
{
    MyLogger.Instance.MethodStart("SomeMethod", $"input={input}");
    
    try
    {
        // 処理
        var result = "処理結果";
        
        MyLogger.Instance.MethodEnd("SomeMethod", $"result={result}");
        return result;
    }
    catch (Exception ex)
    {
        MyLogger.Instance.Error("SomeMethodでエラーが発生しました", ex);
        throw;
    }
}

// カテゴリを指定した場合
public async Task<string> SomeMethodWithCategory(string input)
{
    MyLogger.Instance.MethodStart("SomeMethodWithCategory", $"input={input}", "MyService");
    
    try
    {
        // 処理
        var result = "処理結果";
        
        MyLogger.Instance.MethodEnd("SomeMethodWithCategory", $"result={result}", "MyService");
        return result;
    }
    catch (Exception ex)
    {
        MyLogger.Instance.Error("SomeMethodWithCategoryでエラーが発生しました", ex, "MyService");
        throw;
    }
}
```

### パフォーマンス計測（参考実装）

```csharp
// パフォーマンス計測の例（参考実装）
var stopwatch = System.Diagnostics.Stopwatch.StartNew();

try
{
    // 何らかの重い処理
    await SomeAsyncOperation();
    
    stopwatch.Stop();
    MyLogger.Instance.Performance("処理名", stopwatch.ElapsedMilliseconds);
}
catch (Exception ex)
{
    stopwatch.Stop();
    MyLogger.Instance.Error("エラーメッセージ", ex);
    throw;
}

// カテゴリを指定した場合
var stopwatch2 = System.Diagnostics.Stopwatch.StartNew();

try
{
    // 何らかの重い処理
    await SomeAsyncOperation();
    
    stopwatch2.Stop();
    MyLogger.Instance.Performance("処理名", stopwatch2.ElapsedMilliseconds, "Performance");
}
catch (Exception ex)
{
    stopwatch2.Stop();
    MyLogger.Instance.Error("エラーメッセージ", ex, "Performance");
    throw;
}
```

### 将来的なログイン機能実装

将来的にログイン機能を実装する際は、以下のような形でユーザーIDをログに含めることができます：

- ログイン時にユーザーIDを設定
- ログアウト時にユーザーIDをクリア
- 各操作でユーザーIDが自動的にログに含まれる

### 使用例

プロジェクト内では以下の場所でMyLoggerを使用しています：

- `UserService.cs`: データベース操作のログ
- `UserController.cs`: API呼び出しのログ
- `MyLoggerExample.cs`: 使用例のサンプルコード

### ログ出力の設定

`appsettings.json`でログ出力の設定を行うことができます：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```
