using BlazorApp1.Models;
using BlazorApp1.Services.Interfaces;
using BlazorApp1.Services.State;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TaskLogPageBase : ComponentBase
{
    [Inject] protected IApiService ApiService { get; set; }
    [Inject] protected NavigationManager NavManager { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }
    [Inject] protected ActivityState ActivityState { get; set; }

    protected List<ActivityDto> activities = new();
    protected string newTitle = "";
    protected string newTime = "00:00:00";
    protected bool showTitleError = false;
    protected bool loading = true;

    protected bool editOpen;
    protected int editId;
    protected string editTitle = "";
    protected string editTime = "00:00:00";
    protected bool editShowTitleError = false;

    protected bool deleteOpen;
    protected int deleteId;

    protected override async Task OnInitializedAsync() => await LoadActivities();

    protected async Task LoadActivities()
    {
        loading = true;
        try
        {
            activities = await ApiService.GetActivitiesAsync();
        }
        catch
        {
            Snackbar.Add("خطا در دریافت لیست فعالیت‌ها", Severity.Error);
        }
        loading = false;
    }

    protected async Task AddActivity()
    {
        showTitleError = string.IsNullOrWhiteSpace(newTitle);
        if (showTitleError)
        {
            Snackbar.Add("عنوان را وارد کنید", Severity.Error);
            return;
        }
        if (!TimeSpan.TryParse(newTime, out var ts))
        {
            Snackbar.Add("زمان نامعتبر است", Severity.Error);
            return;
        }
        var dto = new ActivityDto
        {
            Title = newTitle.Trim(),
            TotalSeconds = (int)ts.TotalSeconds
        };
        var ok = await ApiService.CreateActivityAsync(dto);
        Snackbar.Add(ok ? "فعالیت افزوده شد" : "خطا در افزودن",
                     ok ? Severity.Success : Severity.Error);
        if (ok) await LoadActivities();
        newTitle = ""; newTime = "00:00:00"; showTitleError = false;
    }

    protected void OpenEditModal(ActivityDto act)
    {
        editId = act.Id;
        editTitle = act.Title;
        editTime = FormatTime(act.TotalSeconds);
        editShowTitleError = false;
        editOpen = true;
    }

    protected async Task SaveEdit()
    {
        editShowTitleError = string.IsNullOrWhiteSpace(editTitle);
        if (editShowTitleError)
        {
            Snackbar.Add("عنوان نمی‌تواند خالی باشد", Severity.Error);
            return;
        }
        if (!TimeSpan.TryParse(editTime, out var ts))
        {
            Snackbar.Add("زمان نامعتبر است", Severity.Error);
            return;
        }
        var dto = new ActivityDto
        {
            Id = editId,
            Title = editTitle.Trim(),
            TotalSeconds = (int)ts.TotalSeconds
        };
        var ok = await ApiService.UpdateActivityAsync(editId, dto);
        Snackbar.Add(ok ? "ویرایش شد" : "خطا در ویرایش",
                     ok ? Severity.Success : Severity.Error);
        if (ok)
        {
            var itm = activities.FirstOrDefault(a => a.Id == editId);
            if (itm != null) { itm.Title = dto.Title; itm.TotalSeconds = dto.TotalSeconds; }
        }
        editOpen = false;
    }

    protected void OpenDelete(int id) { deleteId = id; deleteOpen = true; }

    protected async Task DeleteActivity()
    {
        var ok = await ApiService.DeleteActivityAsync(deleteId);
        Snackbar.Add(ok ? "حذف شد" : "خطا در حذف",
                     ok ? Severity.Success : Severity.Error);
        if (ok) activities.RemoveAll(a => a.Id == deleteId);
        deleteOpen = false;
    }

    protected Task NavigateToDetails(int id)
    {
        var sel = activities.FirstOrDefault(a => a.Id == id);
        if (sel != null) ActivityState.SetActivity(sel);
        NavManager.NavigateTo($"/TaskLog/Details/{id}");
        return Task.CompletedTask;
    }

    protected static string FormatTime(int seconds)
      => TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
}
