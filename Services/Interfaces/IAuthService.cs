using BlazorApp1.Models;

namespace BlazorApp1.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary> گرفتن پروفایل کاربر از API </summary>
        Task<UserDto?> GetProfileAsync();

        /// <summary> ذخیره توکن و کاربر در LocalStorage </summary>
        Task LoginAsync(string token, UserDto partialUser);

        /// <summary> حذف توکن و کاربر از LocalStorage </summary>
        Task LogoutAsync();

        /// <summary> گرفتن توکن ذخیره‌شده </summary>
        Task<string?> GetTokenAsync();

        /// <summary> گرفتن کاربر ذخیره‌شده </summary>
        Task<UserDto?> GetSavedUserAsync();
    }
}
