namespace BlazorApp1.Helpers;

public static class AdminEndpoints
{
    public const string GetAllUsers = "Admin/AllUsers";
    public static string GetUserDetails(string userId) => $"Admin/UserDetails/{userId}";
    public static string AddUser => "Admin/AddUser";
    public static string UpdateUser(string userId) => $"Admin/UpdateUser/{userId}";
    public static string DeleteUser(string userId) => $"Admin/DeleteUser/{userId}";
    public static string PromoteToAdmin(string userId) => $"Admin/Promote/{userId}";

    public static string ApprovedTasksByUser(string userId) => $"AdminTask/Approved/ByUser/{userId}";
}
