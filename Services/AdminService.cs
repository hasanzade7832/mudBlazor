using BlazorApp1.Helpers;              // AdminEndpoints  
using BlazorApp1.Models;               // UserDto, UserDetailsDto  
using BlazorApp1.Models.Auth;          // RegisterDto  
using BlazorApp1.Models.TaskManagement;
using BlazorApp1.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _http;

        public AdminService(HttpClient http)
        {
            _http = http;
        }

        // 🧑‍🤝‍🧑 Users
        public async Task<List<UserDto>> GetAllUsersAsync() =>
            await _http.GetFromJsonAsync<List<UserDto>>(AdminEndpoints.GetAllUsers) ?? new();

        public async Task<UserDetailsDto?> GetUserDetailsAsync(string userId) =>
            await _http.GetFromJsonAsync<UserDetailsDto>(AdminEndpoints.GetUserDetails(userId));

        public async Task<bool> AddUserAsync(RegisterDto user) =>
            (await _http.PostAsJsonAsync(AdminEndpoints.AddUser, user)).IsSuccessStatusCode;

        public async Task<bool> UpdateUserAsync(string id, UserDto user) =>
            (await _http.PutAsJsonAsync(AdminEndpoints.UpdateUser(id), user)).IsSuccessStatusCode;

        public async Task<bool> DeleteUserAsync(string id) =>
            (await _http.DeleteAsync(AdminEndpoints.DeleteUser(id))).IsSuccessStatusCode;

        public async Task<bool> PromoteToAdminAsync(string id) =>
            (await _http.PutAsync(AdminEndpoints.PromoteToAdmin(id), null)).IsSuccessStatusCode;

        // ✅ Approved Tasks
        public async Task<List<TaskDto>> GetApprovedTasksByUserAsync(string userId) =>
            await _http.GetFromJsonAsync<List<TaskDto>>(AdminEndpoints.ApprovedTasksByUser(userId)) ?? new();

        // 📋 مدیریت تسک‌ها
        public async Task<List<TaskDto>> GetAllTasksAsync() =>
            await _http.GetFromJsonAsync<List<TaskDto>>(AdminEndpoints.GetAllTasks) ?? new();

        public async Task<bool> CreateTaskAsync(CreateTaskDto dto) =>
            (await _http.PostAsJsonAsync(AdminEndpoints.CreateTask, dto)).IsSuccessStatusCode;

        public async Task<bool> EditTaskAsync(int id, EditTaskDto dto) =>
            (await _http.PutAsJsonAsync(AdminEndpoints.EditTask(id), dto)).IsSuccessStatusCode;

        public async Task<bool> DeleteTaskAsync(int id) =>
            (await _http.DeleteAsync(AdminEndpoints.DeleteTask(id))).IsSuccessStatusCode;

        public async Task<bool> ConfirmUserTaskAsync(int userTaskId) =>
            (await _http.PostAsync(AdminEndpoints.ConfirmTask(userTaskId), null)).IsSuccessStatusCode;
    }
}
