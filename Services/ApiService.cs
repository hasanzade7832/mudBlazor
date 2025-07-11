using BlazorApp1.Helpers;             // برای ApiEndpoints  
using BlazorApp1.Models;              // UserDto, ActivityDto, …  
using BlazorApp1.Models.Attendance;
using BlazorApp1.Models.Auth;         // LoginDto, RegisterDto  
using BlazorApp1.Models.TaskManagement;
using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
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

        public async Task<bool> CreateAttendanceAsync(CreateTimeEntryRequest dto)
        {
            var response = await _http.PostAsJsonAsync(ApiEndpoints.CreateAttendance, dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TimeEntry>> GetAttendancesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<TimeEntry>>(ApiEndpoints.GetAttendances);
            return result ?? new();
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var response = await _http.DeleteAsync(ApiEndpoints.DeleteAttendance(id));
            return response.IsSuccessStatusCode;
        }
        public void SetBearerToken(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearBearerToken()
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
        }

        // ApiService.cs
        public async Task<List<ExpenseItemDto>> GetCurrentExpensesAsync() =>
            await _http.GetFromJsonAsync<List<ExpenseItemDto>>(ApiEndpoints.GetCurrentExpenses) ?? new();

        public async Task<ExpenseItemDto?> GetExpenseByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ExpenseItemDto>(ApiEndpoints.GetExpenseById(id));

        public async Task<bool> CreateExpenseAsync(CreateExpenseDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.CreateExpense, dto)).IsSuccessStatusCode;

        public async Task<bool> UpdateExpenseAsync(long id, EditExpenseDto dto) =>
            (await _http.PutAsJsonAsync(ApiEndpoints.EditExpense(id), dto)).IsSuccessStatusCode;

        public async Task<bool> DeleteExpenseAsync(long id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeleteExpense(id))).IsSuccessStatusCode;


        // PettyCash
        public async Task<List<PettyCashItemDto>> GetPettyCashesAsync() =>
            await _http.GetFromJsonAsync<List<PettyCashItemDto>>(ApiEndpoints.GetPettyCashes) ?? new();

        public async Task<PettyCashItemDto?> GetPettyCashByIdAsync(long id) =>
            await _http.GetFromJsonAsync<PettyCashItemDto>(ApiEndpoints.GetPettyCashById(id));

        public async Task<bool> CreatePettyCashAsync(CreatePettyCashDto dto) =>
            (await _http.PostAsJsonAsync(ApiEndpoints.CreatePettyCash, dto)).IsSuccessStatusCode;

        public async Task<bool> DeletePettyCashAsync(long id) =>
            (await _http.DeleteAsync(ApiEndpoints.DeletePettyCash(id))).IsSuccessStatusCode;

        public async Task<string?> UploadReceiptAsync(IBrowserFile file)
        {
            if (file == null) return null;

            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream(10 * 1024 * 1024)); // محدودیت 10MB
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.Name);

            var response = await _http.PostAsync(ApiEndpoints.UploadReceipt, content);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            return json.GetProperty("receiptUrl").GetString();
        }


        public async Task<List<UserTaskDto>> GetMyTasksAsync() =>
    await _http.GetFromJsonAsync<List<UserTaskDto>>(AdminEndpoints.GetMyTasks) ?? new();

        public async Task<bool> CompleteTaskAsync(int userTaskId, int percentComplete)
        {
            var dto = new { PercentComplete = percentComplete };
            var response = await _http.PostAsJsonAsync(AdminEndpoints.CompleteTask(userTaskId), dto);
            return response.IsSuccessStatusCode;
        }




    }
}
