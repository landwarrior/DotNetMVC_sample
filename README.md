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
