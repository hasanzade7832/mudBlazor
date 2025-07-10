namespace BlazorApp1.Helpers;

public static class ApiEndpoints
{
    // 🔐 Authentication
    public const string Login = "Login";
    public const string Register = "Register";

    // 👤 Profile
    public const string GetProfile = "Profile";
    public const string UpdateProfile = "Profile";
    public const string UploadProfilePhoto = "Profile/UploadPhoto";
    public const string DeleteProfilePhoto = "Profile/DeletePhoto";

    // 📋 Activities
    // 📋 Activities
    public const string GetActivities = "api/Activities";
    public const string AddActivity = "api/Activities";
    public static string UpdateActivity(int id) => $"api/Activities/{id}";
    public static string DeleteActivity(int id) => $"api/Activities/{id}";


    // ⏱ Time Records
    // ⏱ Time Records
    public const string GetTimeRecords = "api/TimeRecords";
    public static string GetTimeRecordById(int id) => $"api/TimeRecords/{id}";
    public static string GetTimeRecordsByActivity(int activityId) => $"api/TimeRecords/ByActivity/{activityId}";
    public const string AddTimeRecord = "api/TimeRecords";
    public static string UpdateTimeRecord(int id) => $"api/TimeRecords/{id}";
    public static string DeleteTimeRecord(int id) => $"api/TimeRecords/{id}";


    // 🌐 Internet Purchases & Downloads
    public const string GetPurchases = "Internet/All";
    public const string AddPurchase = "Internet/AddPurchase";
    public static string EditPurchase(int id) => $"Internet/EditPurchase/{id}";
    public static string DeletePurchase(int id) => $"Internet/DeletePurchase/{id}";
    public static string AddDownload(int purchaseId) => $"Internet/AddDownload/{purchaseId}";
    public static string EditDownload(int id) => $"Internet/EditDownload/{id}";
    public static string DeleteDownload(int id) => $"Internet/DeleteDownload/{id}";

    // 🐣 Eggs
    public const string GetEggCounts = "Egg/Counts";
    public const string GetMyEggLogs = "Egg/MyLogs";
    public const string CreateEggLog = "Egg";
    public static string DeleteEggLog(int id) => $"Egg/{id}";
    public static string DecrementEggCount(string userId) => $"Egg/Decrement/{userId}";

    // 📅 Attendance
    public const string CreateAttendance = "TimeEntries";
    public const string GetAttendances = "TimeEntries";
    public static string DeleteAttendance(int id) => $"TimeEntries/{id}";

    /* 🔹 Petty-Cash & Expense -------------------------------------------- */
    public const string GetCurrentExpenses = "api/Expense";
    public static string GetExpenseById(long id) => $"api/Expense/{id}";
    public const string CreateExpense = "api/Expense";
    public static string EditExpense(long id) => $"api/Expense/{id}";
    public static string DeleteExpense(long id) => $"api/Expense/{id}";
    public const string UploadReceipt = "api/Expense/UploadReceipt";

    public const string GetPettyCashes = "api/PettyCash";
    public static string GetPettyCashById(long id) => $"api/PettyCash/{id}";
    public const string CreatePettyCash = "api/PettyCash";
    public static string DeletePettyCash(long id) => $"api/PettyCash/{id}";
    /* ------------------------------------------------------------------- */



}


