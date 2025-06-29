using Blazored.LocalStorage;
using BlazorApp1.Models;
using BlazorApp1.Services.Interfaces;
using System.Net.Http.Json;

namespace BlazorApp1.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        // 📋 گرفتن پروفایل کاربر از سرور
        public async Task<UserDto?> GetProfileAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<UserDto>("api/Profile");
            }
            catch
            {
                return null;
            }
        }

        // 🔐 ذخیره توکن و کاربر در LocalStorage
        public async Task LoginAsync(string token, UserDto partialUser)
        {
            await _localStorage.SetItemAsync("token", token);
            await _localStorage.SetItemAsync("user", partialUser);
        }

        // 🔓 خروج و حذف توکن و اطلاعات کاربر
        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("token");
            await _localStorage.RemoveItemAsync("user");
        }

        // 📦 گرفتن توکن از LocalStorage
        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("token");
        }

        // 📦 گرفتن اطلاعات کاربر ذخیره‌شده
        public async Task<UserDto?> GetSavedUserAsync()
        {
            return await _localStorage.GetItemAsync<UserDto>("user");
        }
    }
}
