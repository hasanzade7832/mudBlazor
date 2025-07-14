using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp1;
using BlazorApp1.Services;
using BlazorApp1.Services.Interfaces;
using Blazored.Toast;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using BlazorApp1.Services.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// آدرس پایه API
//var BASE_URL = "https://localhost:7223/api";
var BASE_URL = "http://localhost:5001/api";


// ثبت local storage برای نگهداری توکن
builder.Services.AddBlazoredLocalStorage();

// 🔐 ثبت DelegatingHandler برای اضافه کردن توکن JWT به هر درخواست
builder.Services.AddTransient<TokenHandler>();

builder.Services.AddMudServices();

builder.Services.AddMudServices();

// ثبت HttpClient و متصل کردن TokenHandler به HttpClientHandler
builder.Services.AddScoped(sp =>
{
    var tokenHandler = sp.GetRequiredService<TokenHandler>();
    tokenHandler.InnerHandler = new HttpClientHandler(); // مهم: بدون این خط خطای inner handler می‌گیری

    return new HttpClient(tokenHandler)
    {
        BaseAddress = new Uri(BASE_URL)
    };
});

// 🧩 سرویس‌های اصلی پروژه
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ActivityState>();


// 🔒 احراز هویت و نقش‌ها
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// 📡 SignalR
builder.Services.AddSingleton<SignalRService>();

// 🔔 Toast notifications
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
