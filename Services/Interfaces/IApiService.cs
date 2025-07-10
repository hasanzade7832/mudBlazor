using BlazorApp1.Models;           // برای UserDto, ActivityDto, …  
using BlazorApp1.Models.Auth;      // برای LoginDto, RegisterDto  
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;             // برای MultipartFormDataContent
namespace BlazorApp1.Services.Interfaces
{
    public interface IApiService
    {
        // 🔐 Authentication
        Task<LoginResponse?> LoginAsync(LoginDto model);
        Task<string> RegisterAsync(RegisterDto model);

        // 👤 Profile
        Task<UserDto?> GetProfileAsync();
        Task<bool> UpdateProfileAsync(UserDto profile);
        Task<string?> UploadProfilePhotoAsync(MultipartFormDataContent content);
        Task<bool> DeleteProfilePhotoAsync();

        // 📋 Activities
        Task<List<ActivityDto>> GetActivitiesAsync();
        Task<bool> CreateActivityAsync(ActivityDto dto);
        Task<bool> UpdateActivityAsync(int id, ActivityDto dto);
        Task<bool> DeleteActivityAsync(int id);

        // ⏱ Time Records
        Task<List<TimeRecordDto>> GetTimeRecordsAsync();
        Task<TimeRecordDto?> GetTimeRecordByIdAsync(int id);
        Task<List<TimeRecordDto>> GetTimeRecordsByActivityAsync(int activityId);
        Task<bool> CreateTimeRecordAsync(TimeRecordDto dto);
        Task<bool> UpdateTimeRecordAsync(int id, TimeRecordDto dto);
        Task<bool> DeleteTimeRecordAsync(int id);

        // 🌐 Internet
        Task<List<PurchaseDto>> GetPurchasesAsync();
        Task<bool> CreatePurchaseAsync(PurchaseDto dto);
        Task<bool> UpdatePurchaseAsync(int id, PurchaseDto dto);
        Task<bool> DeletePurchaseAsync(int id);
        Task<bool> AddDownloadAsync(int purchaseId, DownloadDto dto);
        Task<bool> UpdateDownloadAsync(int id, DownloadDto dto);
        Task<bool> DeleteDownloadAsync(int id);

        // 🐣 Eggs
        Task<List<EggLogDto>> GetMyEggLogsAsync();
        Task<bool> CreateEggLogAsync(EggLogDto dto);
        Task<bool> DeleteEggLogAsync(int id);
        Task<bool> DecrementEggAsync(string userId);

        // 📅 Attendance
        Task<bool> CreateAttendanceAsync(AttendanceDto dto);
        Task<List<AttendanceDto>> GetAttendancesAsync();
        Task<bool> DeleteAttendanceAsync(int id);

        /// <summary>تنظیم هدر Authorization برای توکن</summary>
        void SetBearerToken(string token);

        /// <summary>پاک کردن هدر Authorization</summary>
        void ClearBearerToken();

        // IApiService.cs
        Task<List<ExpenseItemDto>> GetCurrentExpensesAsync();
        Task<ExpenseItemDto?> GetExpenseByIdAsync(long id);
        Task<bool> CreateExpenseAsync(CreateExpenseDto dto);
        Task<bool> UpdateExpenseAsync(long id, EditExpenseDto dto);
        Task<bool> DeleteExpenseAsync(long id);

        Task<List<PettyCashItemDto>> GetPettyCashesAsync();
        Task<PettyCashItemDto?> GetPettyCashByIdAsync(long id);
        Task<bool> CreatePettyCashAsync(CreatePettyCashDto dto);
        Task<bool> DeletePettyCashAsync(long id);

        Task<string?> UploadReceiptAsync(IBrowserFile file);


    }
}
