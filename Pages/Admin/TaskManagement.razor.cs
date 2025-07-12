using BlazorApp1.Models.TaskManagement;
using BlazorApp1.Models.Auth;
using BlazorApp1.Services.Interfaces;
using BlazorApp1.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApp1.Pages.Admin;

/// <summary>
/// تمام منطق صفحه «مدیریت تسک‌ها».
public partial class TaskManagementBase : ComponentBase
{
    // ---------- DI ----------
    [Inject] public IAdminService AdminService { get; set; } = default!;
    [Inject] public SignalRService SignalRService { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    // ---------- State ----------
    protected List<TaskDto> Tasks { get; private set; } = new();
    protected List<SimpleUserDto> Users { get; private set; } = new();

    protected bool ShowModal { get; private set; }
    protected string ModalMode { get; private set; } = "add";      // add | edit
    protected bool ModalLoading { get; private set; }

    protected int? EditingTaskId;
    protected CreateTaskDto SelectedTask = new();

    // ---------- lifecycle ----------
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();

        // گوش دادن به SignalR
        SignalRService.OnTaskCompletedByUser((_, _) => RefreshTable("یک کاربر پیشرفت تسک را ثبت کرد"));
        SignalRService.OnTaskUpdated(_ => RefreshTable("تسک به‌روزرسانی شد"));
        SignalRService.OnTaskDeleted(_ => RefreshTable("تسک حذف شد"));
        SignalRService.OnTaskAssigned(_ => RefreshTable());
        SignalRService.OnTaskConfirmed(_ => RefreshTable());
    }

    // ---------- Data ----------
    private async Task LoadDataAsync()
    {
        // تسک‌ها
        Tasks = await AdminService.GetAllTasksAsync();
        // کاربران
        Users = (await AdminService.GetAllUsersAsync())
                    .Select(u => new SimpleUserDto { Id = u.Id, UserName = u.UserName })
                    .ToList();
    }

    private async Task RefreshTable(string? toast = null)
    {
        await LoadTasksOnlyAsync();
        if (!string.IsNullOrWhiteSpace(toast))
            Snackbar.Add(toast, Severity.Info);
    }

    private async Task LoadTasksOnlyAsync()
    {
        Tasks = await AdminService.GetAllTasksAsync();
        StateHasChanged();
    }

    // ---------- Modal ----------
    protected void OpenAddModal()
    {
        ModalMode = "add";
        EditingTaskId = null;
        SelectedTask = new();
        ShowModal = true;
    }

    protected void OpenEditModal((TaskDto task, string userId) data)
    {
        var (task, userId) = data;

        ModalMode = "edit";
        EditingTaskId = task.Id;
        SelectedTask = new()
        {
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            UserIds = new() { userId }
        };
        ShowModal = true;
    }

    protected void CloseModal() => ShowModal = false;

    // ---------- CRUD ----------
    protected async Task HandleSubmitAsync(CreateTaskDto dto)
    {
        ModalLoading = true; StateHasChanged();

        bool ok = ModalMode == "add"
            ? await AdminService.CreateTaskAsync(dto)
            : (EditingTaskId.HasValue && await AdminService.EditTaskAsync(
                    EditingTaskId.Value,
                    new EditTaskDto
                    {
                        Title = dto.Title,
                        Description = dto.Description,
                        Deadline = dto.Deadline,
                        UserIds = dto.UserIds
                    }));

        Snackbar.Add(ok ? "تسک ذخیره شد" : "خطا در ذخیره‌سازی تسک",
                     ok ? Severity.Success : Severity.Error);

        ModalLoading = false;

        if (ok) { ShowModal = false; await LoadTasksOnlyAsync(); }
    }

    protected async Task DeleteTaskAsync(int taskId)
    {
        if (!await JS.InvokeAsync<bool>("confirm", "آیا از حذف این تسک مطمئن هستید؟"))
            return;

        var ok = await AdminService.DeleteTaskAsync(taskId);

        Snackbar.Add(ok ? "تسک حذف شد" : "خطا در حذف تسک",
                     ok ? Severity.Success : Severity.Error);

        if (ok) await LoadTasksOnlyAsync();
    }

    protected async Task ConfirmTaskAsync(int userTaskId)
    {
        var ok = await AdminService.ConfirmUserTaskAsync(userTaskId);
        Snackbar.Add(ok ? "تسک تأیید شد" : "خطا در تأیید تسک",
                     ok ? Severity.Success : Severity.Error);

        if (ok) await LoadTasksOnlyAsync();
    }
}
