using MyMvcApp.DAL;
using Microsoft.EntityFrameworkCore;
using MyMvcApp; // 追加: RequestLoggingMiddleware 用
using MyMvcApp.Services; // 追加: UserService 用

var builder = WebApplication.CreateBuilder(args);

// DbContext を DI コンテナに登録（SQLite 使用）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Service層の依存性注入を追加
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    /*
    HTTP Strict Transport Security プロトコル (HSTS)
    OWASP によると、HTTP Strict Transport Security (HSTS) は、応答ヘッダーを使って Web アプリによって指定されるオプトイン セキュリティ拡張機能です。 HSTS をサポートするブラウザーがこのヘッダーを受け取ると、次のようになります。

    - ブラウザーに、HTTP 経由の通信を送信できないようにするドメインの構成が保存されます。 ブラウザーでは、すべての通信が強制的に HTTPS 経由で実行されます。
    - ブラウザーによって、ユーザーが信頼されていない証明書や無効な証明書を使えなくなります。 ユーザーがそのような証明書を一時的に信頼できるようにするプロンプトがブラウザーで無効になります。

    HSTS はクライアントによって適用されるため、いくつかの制限事項があります。

    - クライアントで HSTS がサポートされている必要があります。
    - HSTS では、HSTS ポリシーを確立するために、少なくとも 1つの成功した HTTPS 要求が必要です。
    - アプリケーションではすべての HTTP 要求をチェックし、その HTTP 要求をリダイレクトまたは拒否する必要があります。
    */
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ここでリクエストロギングミドルウェアを追加
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}/{id2?}");

app.Run();
