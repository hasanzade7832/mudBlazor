using BlazorApp1.Models.TaskManagement;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp1.Pages.MyTasks;

public partial class MyTaskCard : ComponentBase
{
    [Parameter] public UserTaskDto? UserTask { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    private int draftPercent;

    protected override void OnInitialized()
    {
        draftPercent = UserTask?.PercentComplete ?? 0;
    }

    private int Percent
    {
        get => draftPercent;
        set
        {
            if (draftPercent != value)
            {
                draftPercent = value;
                StateHasChanged();
            }
        }
    }

    private string TaskTitle => UserTask?.TaskItem?.Title ?? "(بدون عنوان)";
    private string TaskDescription => UserTask?.TaskItem?.Description ?? "";
    private DateTime TaskDeadline => UserTask?.TaskItem?.Deadline ?? DateTime.Now;
    private DateTime TaskCreatedAt => UserTask?.TaskItem?.CreatedAt ?? DateTime.Now;

    private bool CanSave => Percent != (UserTask?.PercentComplete ?? 0)
                            && !(UserTask?.IsConfirmedByAdmin ?? true)
                            && !IsDeadlinePassed;
    private bool CanEdit => !(UserTask?.IsConfirmedByAdmin ?? true) && !IsDeadlinePassed;
    private bool IsDeadlinePassed => TaskDeadline < DateTime.Now;

    private string RemainBadgeText
    {
        get
        {
            var rem = (TaskDeadline - DateTime.Now).TotalDays;
            if (rem <= 0) return "مهلت تمام";
            return $"{Math.Ceiling(rem)} روز مانده";
        }
    }
    private Color RemainBadgeColor
    {
        get
        {
            var rem = (TaskDeadline - DateTime.Now).TotalDays;
            var total = (TaskDeadline - TaskCreatedAt).TotalDays;
            var pct = total > 0 ? ((total - rem) / total) * 100 : 100;
            if (pct >= 80) return Color.Error;
            if (pct >= 30) return Color.Warning;
            return Color.Success;
        }
    }
    private string StatusBadgeText
    {
        get
        {
            if (UserTask?.IsConfirmedByAdmin == true) return "تأیید شد";
            if (UserTask?.IsCompletedByUser == true) return "در انتظار تأیید";
            return "در حال انجام";
        }
    }
    private Color StatusBadgeColor
    {
        get
        {
            if (UserTask?.IsConfirmedByAdmin == true) return Color.Success;
            if (UserTask?.IsCompletedByUser == true) return Color.Warning;
            return Color.Info;
        }
    }

    private string StatusChipTopClass =>
        $"badge-chip-top{(UserTask?.IsCompletedByUser == true && !(UserTask?.IsConfirmedByAdmin ?? false) ? " pulse-badge" : "")}";

    private string CardClass
    {
        get
        {
            if (UserTask?.IsConfirmedByAdmin == true)
                return "mytask-card mx-auto completed";
            if (IsDeadlinePassed)
                return "mytask-card mx-auto expired";
            return "mytask-card mx-auto";
        }
    }

    private Task OnPercentChanged(int v)
    {
        Percent = v;
        return Task.CompletedTask;
    }

    private async Task SaveProgress()
    {
        if (UserTask == null) return;
        var dto = new CompleteTaskDto { PercentComplete = Percent };
        var ok = await AdminService.CompleteTaskAsync(UserTask.Id, dto);
        if (ok && OnSaved.HasDelegate)
            await OnSaved.InvokeAsync();
    }
}
