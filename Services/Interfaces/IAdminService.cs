using BlazorApp1.Models;               // UserDto, UserDetailsDto  
using BlazorApp1.Models.Auth;          // RegisterDto  

using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services.Interfaces
{
    public interface IAdminService
    {
        // 🧑‍🤝‍🧑 Users
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDetailsDto?> GetUserDetailsAsync(string userId);
        Task<bool> AddUserAsync(RegisterDto user);
        Task<bool> UpdateUserAsync(string id, UserDto user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> PromoteToAdminAsync(string id);

        // ✅ Approved Tasks
        Task<List<TaskDto>> GetApprovedTasksByUserAsync(string userId);
    }
}
