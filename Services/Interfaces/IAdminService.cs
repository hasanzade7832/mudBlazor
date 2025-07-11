using BlazorApp1.Models;
using BlazorApp1.Models.Auth;
using BlazorApp1.Models.TaskManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services.Interfaces
{
    public interface IAdminService
    {
        // 🧑‍🤝‍🧑 مدیریت کاربران
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDetailsDto?> GetUserDetailsAsync(string userId);
        Task<bool> AddUserAsync(RegisterDto user);
        Task<bool> UpdateUserAsync(string id, UserDto user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> PromoteToAdminAsync(string id);

        // ✅ دریافت تسک‌های تأییدشده
        Task<List<TaskDto>> GetApprovedTasksByUserAsync(string userId);

        // 📋 مدیریت تسک‌ها (AdminTask)
        Task<List<TaskDto>> GetAllTasksAsync();
        Task<bool> CreateTaskAsync(CreateTaskDto dto);
        Task<bool> EditTaskAsync(int id, EditTaskDto dto);
        Task<bool> DeleteTaskAsync(int id);
        Task<bool> ConfirmUserTaskAsync(int userTaskId);
    }
}
