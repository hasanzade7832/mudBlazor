using System.Linq.Expressions;
using BlazorApp1.Components;
using BlazorApp1.Components.DynamicTable;
using BlazorApp1.Models.Attendance;
using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp1.Pages.Attendance;

public partial class Attendance : ComponentBase, IDisposable
{
    /* ---------- تزریق سرویس‌ها ---------- */
    [Inject] private IApiService ApiService { get; set; } = default!;
    //[Inject] private IDialogService DialogService { get; set; } = default!;

    /* ---------- فیلدها ---------- */
    private DateTime _now = DateTime.Now;
    private DateTime? StartTime = null;
    private DateTime? StopTime = null;
    private string Tasks = string.Empty;
    private List<TimeEntry> Entries = new();

    private System.Timers.Timer? _clockTimer;
    private System.Timers.Timer? _elapsedTimer;

    /* ---------- چرخهٔ حیات ---------- */
    protected override async Task OnInitializedAsync()
    {
        await LoadEntries();

        _clockTimer = new(1000);
        _clockTimer.Elapsed += (_, _) =>
        {
            _now = DateTime.Now;
            InvokeAsync(StateHasChanged);
        };
        _clockTimer.Start();
    }

    public void Dispose()
    {
        _clockTimer?.Dispose();
        _elapsedTimer?.Dispose();
    }

    /* ---------- داده ---------- */
    private async Task LoadEntries() =>
        Entries = await ApiService.GetAttendancesAsync();

    /* ---------- رویدادهای UI ---------- */
    private void HandleCheckIn()
    {
        StartTime = DateTime.Now;

        _elapsedTimer?.Dispose();
        _elapsedTimer = new(1000);
        _elapsedTimer.Elapsed += (_, _) => InvokeAsync(StateHasChanged);
        _elapsedTimer.Start();
    }

    private async Task HandleCheckOut()
    {
        if (!StartTime.HasValue) return;

        StopTime = DateTime.Now;
        _elapsedTimer?.Dispose();

        await ShowExitDialogAsync();
    }

    /* ---------- دیالوگ ---------- */
    private async Task ShowExitDialogAsync()
    {
        var parameters = new DialogParameters
        {
            ["Title"] = "ثبت خروج",
            ["OkText"] = "ثبت",
            ["CancelText"] = "انصراف",
            ["ChildContent"] = (RenderFragment)(builder =>
            {
                builder.OpenComponent<MudTextField<string>>(0);
                builder.AddAttribute(1, "Lines", 3);
                builder.AddAttribute(2, "FullWidth", true);
                builder.AddAttribute(3, "Immediate", true);
                builder.AddAttribute(4, "Placeholder", "کارهای انجام‌شده ...");
                builder.AddAttribute(5, "Text", BindConverter.FormatValue(Tasks));
                builder.AddAttribute(6, "TextChanged",
                    EventCallback.Factory.Create<string>(this, v => Tasks = v));
                builder.AddAttribute(7, "TextExpression", (Expression<Func<string>>)(() => Tasks));
                builder.CloseComponent();
            })
        };

        /* متد توصیه‌شدهٔ جدید → ShowAsync */
        var dialogRef = await DialogService.ShowAsync<AllModal>(string.Empty, parameters);
        var result = await dialogRef.Result;

        if (!result.Canceled)
            await PersistExitAsync();
        else
            StopTime = null; // لغو شد
    }

    /* ---------- منطق کسب‌وکار ---------- */
    private async Task PersistExitAsync()
    {
        if (!StartTime.HasValue || !StopTime.HasValue) return;

        var diff = StopTime.Value - StartTime.Value;
        var duration = $"{(int)diff.TotalHours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";

        var model = new CreateTimeEntryRequest
        {
            CheckIn = StartTime.Value,
            CheckOut = StopTime.Value,
            Duration = duration,
            Tasks = string.IsNullOrWhiteSpace(Tasks) ? "—" : Tasks,
            ShamsiDate = _now.ToString("yyyy/MM/dd")
        };

        if (await ApiService.CreateAttendanceAsync(model))
        {
            await LoadEntries();
            StartTime = null;
            StopTime = null;
            Tasks = string.Empty;
        }
    }

    /* ---------- مقادیر محاسبه‌شده ---------- */
    private string ElapsedStr =>
        StartTime.HasValue
            ? (DateTime.Now - StartTime.Value).ToString(@"hh\:mm\:ss")
            : "00:00:00";

    private string TotalDuration
    {
        get
        {
            var total = TimeSpan.Zero;
            foreach (var e in Entries)
                if (TimeSpan.TryParse(e.Duration, out var t))
                    total += t;

            return $"{(int)total.TotalHours:D2}:{total.Minutes:D2}:{total.Seconds:D2}";
        }
    }

    private List<DynamicTable<TimeEntry>.TableColumn> Columns => new()
    {
        new("tasks",      "کارهای امروز", e => b => b.AddContent(0, e.Tasks)),
        new("duration",   "مدت زمان",    e => b => b.AddContent(0, e.Duration)),
        new("checkOut",   "خروج",         e => b => b.AddContent(0, FormatTime(e.CheckOut))),
        new("checkIn",    "ورود",         e => b => b.AddContent(0, FormatTime(e.CheckIn))),
        new("shamsiDate", "تاریخ",        e => b => b.AddContent(0, e.ShamsiDate))
    };

    /* ---------- ابزار ---------- */
    private static string FormatTime(DateTime? time) =>
        time.HasValue ? time.Value.ToString("HH:mm:ss") : "—";
}
