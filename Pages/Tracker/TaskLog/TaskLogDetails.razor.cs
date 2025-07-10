using BlazorApp1.Models;
using BlazorApp1.Services.Interfaces;
using BlazorApp1.Services.State;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TaskLogDetailsBase : ComponentBase
{
    [Parameter] public int id { get; set; }

    [Inject] protected IApiService ApiService { get; set; }
    [Inject] protected NavigationManager NavManager { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }
    [Inject] protected ActivityState ActivityState { get; set; }

    protected List<ActivityDto> _records = new();
    protected int _seconds = 0;
    protected bool _isRunning = false;
    protected string _recordTitle = "";
    protected System.Threading.Timer? _timer;

    protected bool _isEditDialogOpen = false;
    protected int? _editingRecordId = null;
    protected string _editDialogTitle = "";
    protected string _editDialogDuration = "00:00:00";

    // جمع مدت کل رکوردها و آپدیت دیتابیس
    protected async Task LoadRecordsAndUpdateTotal()
    {
        var data = await ApiService.GetTimeRecordsByActivityAsync(id);
        _records = data.OrderByDescending(r => r.Id)
            .Select(r => new ActivityDto
            {
                Id = r.Id,
                Title = r.Title,
                TotalSeconds = ToSeconds(r.Duration)
            }).ToList();

        var sum = _records.Sum(x => x.TotalSeconds);
        if (ActivityState.SelectedActivity != null)
        {
            ActivityState.SelectedActivity.TotalSeconds = sum;
            await ApiService.UpdateActivityAsync(id, new ActivityDto
            {
                Id = id,
                Title = ActivityState.SelectedActivity.Title,
                TotalSeconds = sum
            });
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (ActivityState.SelectedActivity is null)
        {
            var all = await ApiService.GetActivitiesAsync();
            var found = all.FirstOrDefault(a => a.Id == id);
            if (found != null)
                ActivityState.SetActivity(found);
        }
        await LoadRecordsAndUpdateTotal();
    }

    protected void ToggleTimer()
    {
        _isRunning = !_isRunning;
        if (_isRunning)
        {
            _timer = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() =>
                {
                    _seconds++;
                    StateHasChanged();
                });
            }, null, 1000, 1000);
        }
        else
            _timer?.Dispose();
    }

    protected async Task SaveRecord()
    {
        if (string.IsNullOrWhiteSpace(_recordTitle))
        {
            Snackbar.Add("عنوان رکورد الزامی است", Severity.Error);
            return;
        }
        var now = DateTime.Now;
        var dto = new TimeRecordDto
        {
            ActivityId = id,
            Title = _recordTitle.Trim(),
            CheckIn = now,
            CheckOut = now.AddSeconds(_seconds),
            Duration = FormatTime(_seconds)
        };
        if (await ApiService.CreateTimeRecordAsync(dto))
        {
            _recordTitle = "";
            _seconds = 0;
            await LoadRecordsAndUpdateTotal();
            Snackbar.Add("رکورد ذخیره شد", Severity.Success);
        }
        else
            Snackbar.Add("خطا در ذخیره رکورد", Severity.Error);
    }

    protected void HandleEdit(ActivityDto rec)
    {
        _editingRecordId = rec.Id;
        _editDialogTitle = rec.Title;
        _editDialogDuration = FormatTime(rec.TotalSeconds);
        _isEditDialogOpen = true;
    }

    protected async Task SaveEditDialog()
    {
        if (_editingRecordId == null ||
            string.IsNullOrWhiteSpace(_editDialogTitle) ||
            !TimeSpan.TryParse(_editDialogDuration, out var ts))
        {
            Snackbar.Add("عنوان یا فرمت زمان صحیح نیست", Severity.Error);
            return;
        }
        var dto = new TimeRecordDto
        {
            Id = _editingRecordId.Value,
            ActivityId = id,
            Title = _editDialogTitle.Trim(),
            CheckIn = DateTime.Now.AddSeconds(-ts.TotalSeconds),
            CheckOut = DateTime.Now,
            Duration = _editDialogDuration
        };
        if (await ApiService.UpdateTimeRecordAsync(dto.Id, dto))
        {
            _isEditDialogOpen = false;
            await LoadRecordsAndUpdateTotal();
            Snackbar.Add("ویرایش انجام شد", Severity.Success);
        }
        else
            Snackbar.Add("خطا در ویرایش رکورد", Severity.Error);
    }

    protected async Task DeleteRecord(int recordId)
    {
        if (await ApiService.DeleteTimeRecordAsync(recordId))
        {
            await LoadRecordsAndUpdateTotal();
            Snackbar.Add("رکورد حذف شد", Severity.Success);
        }
        else
            Snackbar.Add("خطا در حذف رکورد", Severity.Error);
    }

    protected void GoBack() => NavManager.NavigateTo("/TaskLog");

    protected static string FormatTime(int sec)
      => TimeSpan.FromSeconds(sec).ToString(@"hh\:mm\:ss");

    protected static int ToSeconds(string str)
      => TimeSpan.TryParse(str, out var ts) ? (int)ts.TotalSeconds : 0;
}
