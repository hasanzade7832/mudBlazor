namespace BlazorApp1.Helpers
{
    public static class AdminEndpoints
    {
        // مدیریت کاربران
        public const string GetAllUsers = "api/Admin/AllUsers";
        public static string GetUserDetails(string userId) => $"api/Admin/UserDetails/{userId}";
        public static string AddUser = "api/Admin/AddUser";
        public static string UpdateUser(string userId) => $"api/Admin/UpdateUser/{userId}";
        public static string DeleteUser(string userId) => $"api/Admin/DeleteUser/{userId}";
        public static string PromoteToAdmin(string userId) => $"api/Admin/Promote/{userId}";

        // تسک‌ها
        public const string GetAllTasks = "api/AdminTask/All";
        public const string CreateTask = "api/AdminTask/Create";
        public static string EditTask(int id) => $"api/AdminTask/Edit/{id}";
        public static string DeleteTask(int id) => $"api/AdminTask/Delete/{id}";
        public static string ConfirmTask(int userTaskId) => $"api/AdminTask/Confirm/{userTaskId}";
        public static string ApprovedTasksByUser(string userId) => $"api/AdminTask/Approved/ByUser/{userId}";

        // تسک‌های کاربر
        public const string GetMyTasks = "api/UserTask/MyTasks";
        public static string CompleteTask(int userTaskId) => $"api/UserTask/Complete/{userTaskId}";
    }
}
