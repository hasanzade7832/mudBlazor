using BlazorApp1.Models.TaskManagement;
using BlazorApp1.Components.Alert;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using BlazorApp1.Services.Interfaces;
using BlazorApp1.Services;
using Microsoft.JSInterop;

namespace BlazorApp1.Pages.MyTasks;

public partial class MyTasksBase : ComponentBase
{
    [Inject] public IAdminService AdminService { get; set; } = default!;
    [Inject] public SignalRService SignalRService { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    protected List<UserTaskDto> UserTasks { get; set; } = new();
    protected bool IsLoading { get; set; } = true;
    protected string? AlertMessage { get; set; }
    protected string? AlertTaskTitle { get; set; }
    protected Severity AlertType { get; set; } = Severity.Info;
    protected bool ShowAlert { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadTasks();

        SignalRService.OnTaskAssigned(taskId => OnSignalEvent(Severity.Info, "یک تسک جدید به شما اختصاص یافت", taskId));
        SignalRService.OnTaskUpdated(taskId => OnSignalEvent(Severity.Info, "یک تسک ویرایش شد", taskId));
        SignalRService.OnTaskDeleted(taskId => OnSignalEvent(Severity.Info, "یک تسک حذف شد", taskId));
        SignalRService.OnTaskConfirmed(userTaskId => OnSignalEvent(Severity.Success, "تسک شما توسط مدیر تایید شد!", userTaskId));
    }

    protected void OnSignalEvent(Severity type, string msg, int? relatedId = null)
    {
        InvokeAsync(async () =>
        {
            await LoadTasks();
            string? title = null;

            if (relatedId.HasValue)
            {
                var ut = UserTasks.FirstOrDefault(x => x.Id == relatedId.Value);
                if (ut?.TaskItem?.Title != null)
                    title = $"«{ut.TaskItem.Title}»";
                else
                {
                    var ut2 = UserTasks.FirstOrDefault(x => x.TaskItem?.Id == relatedId.Value);
                    if (ut2?.TaskItem?.Title != null)
                        title = $"«{ut2.TaskItem.Title}»";
                }
            }

            Show(type, msg, title);
            StateHasChanged();
        });
    }

    protected async Task LoadTasks()
    {
        IsLoading = true;
        StateHasChanged();
        try
        {
            var list = await AdminService.GetMyTasksAsync();
            UserTasks = list?
                .Where(ut => ut != null && ut.TaskItem != null)
                .OrderByDescending(ut => ut.TaskItem?.CreatedAt ?? DateTime.MinValue)
                .ToList() ?? new();
        }
        catch
        {
            Show(Severity.Error, "دریافت لیست تسک‌ها با خطا مواجه شد.", null);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected void Show(Severity type, string msg, string? taskTitle = null)
    {
        AlertType = type;
        AlertMessage = msg;
        AlertTaskTitle = taskTitle;
        ShowAlert = true;
    }

    protected async Task ReloadTasks()
    {
        await LoadTasks();
        Show(Severity.Success, "پیشرفت ثبت شد!", null);
    }
}
