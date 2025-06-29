using BlazorApp1.Helpers;             // برای ApiEndpoints  
using BlazorApp1.Models;              // UserDto, ActivityDto, …  
using BlazorApp1.Models.Auth;         // LoginDto, RegisterDto  
using BlazorApp1.Services.Interfaces;
using System.Net.Http;                // MultipartFormDataContent  
using System.Net.Http.Headers;
using System.Net.Http.Json;           // PostAsJsonAsync, GetFromJsonAsync  
namespace BlazorApp1.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;
        public ApiService(HttpClient http) => _http = http;

        // 🔐 Authentication
        public async Task<LoginResponse?> LoginAsync(LoginDto model)
        {
            var response = await _http.PostAsJsonAsync(ApiEndpoints.Login, model);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }


        public async Task<string> RegisterAsync(RegisterDto model) =>
            await (await _http.PostAsJsonAsync(ApiEndpoints.Register, model))
                .Content.ReadAsStringAsync();

        // 👤 Profile
        public async Task<UserDto?> GetProfileAsync() =>
            await _http.GetFromJsonAsync<UserDto>(ApiEndpoints.GetProfile);

        public async Task<bool> UpdateProfileAsync(UserDto profile)
        {
            var res = await _http.PutAsJsonAsync(ApiEndpoints.UpdateProfile, profile);
            return res.IsSuccessStatusCode;
        }

        public async Task<string?> UploadProfilePhotoAsync(MultipartFormDataContent content)
        {
            var res = await _http.PostAsync(ApiEndpoints.UploadProfilePhoto, content);
            return await res.Content.ReadAsStringAsync();
        }

        public async Task<bool> DeleteProfilePhotoAsync()
        {
            var res = await _http.DeleteAsync(ApiEndpoints.DeleteProfilePhoto);
            return res.IsSuccessStatusCode;
        }

        // 📋 Activities
        public async Task<List<ActivityDto>> GetActivitiesAsync() =>
            await _http.GetFromJsonAsync<List<ActivityDto>>(ApiEndpoints.GetActivities)
            ?? new();

        public async Task<bool> CreateActivityAsync(ActivityDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.AddActivity, dto))
            .IsSuccessStatusCode;

        public async Task<bool> UpdateActivityAsync(int id, ActivityDto dto) =>
            (await _http.PutAsJsonAsync(ApiEndpoints.UpdateActivity(id), dto))
            .IsSuccessStatusCode;

        public async Task<bool> DeleteActivityAsync(int id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeleteActivity(id)))
            .IsSuccessStatusCode;

        // ⏱ Time Records
        public async Task<List<TimeRecordDto>> GetTimeRecordsAsync() =>
            await _http.GetFromJsonAsync<List<TimeRecordDto>>(ApiEndpoints.GetTimeRecords)
            ?? new();

        public async Task<TimeRecordDto?> GetTimeRecordByIdAsync(int id) =>
            await _http.GetFromJsonAsync<TimeRecordDto>(ApiEndpoints.GetTimeRecordById(id));

        public async Task<List<TimeRecordDto>> GetTimeRecordsByActivityAsync(int activityId) =>
            await _http.GetFromJsonAsync<List<TimeRecordDto>>(ApiEndpoints.GetTimeRecordsByActivity(activityId))
            ?? new();

        public async Task<bool> CreateTimeRecordAsync(TimeRecordDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.AddTimeRecord, dto))
            .IsSuccessStatusCode;

        public async Task<bool> UpdateTimeRecordAsync(int id, TimeRecordDto dto) =>
            (await _http.PutAsJsonAsync(ApiEndpoints.UpdateTimeRecord(id), dto))
            .IsSuccessStatusCode;

        public async Task<bool> DeleteTimeRecordAsync(int id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeleteTimeRecord(id)))
            .IsSuccessStatusCode;

        // 🌐 Internet
        public async Task<List<PurchaseDto>> GetPurchasesAsync() =>
            await _http.GetFromJsonAsync<List<PurchaseDto>>(ApiEndpoints.GetPurchases)
            ?? new();

        public async Task<bool> CreatePurchaseAsync(PurchaseDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.AddPurchase, dto))
            .IsSuccessStatusCode;

        public async Task<bool> UpdatePurchaseAsync(int id, PurchaseDto dto) =>
            (await _http.PutAsJsonAsync(ApiEndpoints.EditPurchase(id), dto))
            .IsSuccessStatusCode;

        public async Task<bool> DeletePurchaseAsync(int id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeletePurchase(id)))
            .IsSuccessStatusCode;

        public async Task<bool> AddDownloadAsync(int purchaseId, DownloadDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.AddDownload(purchaseId), dto))
            .IsSuccessStatusCode;

        public async Task<bool> UpdateDownloadAsync(int id, DownloadDto dto) =>
            (await _http.PutAsJsonAsync(ApiEndpoints.EditDownload(id), dto))
            .IsSuccessStatusCode;

        public async Task<bool> DeleteDownloadAsync(int id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeleteDownload(id)))
            .IsSuccessStatusCode;

        // 🐣 Eggs
        public async Task<List<EggLogDto>> GetMyEggLogsAsync() =>
            await _http.GetFromJsonAsync<List<EggLogDto>>(ApiEndpoints.GetMyEggLogs)
            ?? new();

        public async Task<bool> CreateEggLogAsync(EggLogDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.CreateEggLog, dto))
            .IsSuccessStatusCode;

        public async Task<bool> DeleteEggLogAsync(int id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeleteEggLog(id)))
            .IsSuccessStatusCode;

        public async Task<bool> DecrementEggAsync(string userId) =>
            (await _http.PostAsync(ApiEndpoints.DecrementEggCount(userId), null))
            .IsSuccessStatusCode;

        // 📅 Attendance
        public async Task<bool> CreateAttendanceAsync(AttendanceDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.CreateAttendance, dto))
            .IsSuccessStatusCode;

        public async Task<List<AttendanceDto>> GetAttendancesAsync() =>
            await _http.GetFromJsonAsync<List<AttendanceDto>>(ApiEndpoints.GetAttendances)
            ?? new();

        public async Task<bool> DeleteAttendanceAsync(int id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeleteAttendance(id)))
            .IsSuccessStatusCode;

        public void SetBearerToken(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearBearerToken()
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
        }
    }
}
