using BlazorApp1.Components;
using BlazorApp1.Components.DynamicTable;
using BlazorApp1.Components.Modal;
using BlazorApp1.Models;
using BlazorApp1.Services.Interfaces;
using BlazorApp1.Services.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TaskLogDetailsBase : ComponentBase, IDisposable
{
    /* ------------ پارامتر صفحه ------------ */
    [Parameter] public int id { get; set; }

    /* ------------ تزریق سرویس‌ها ------------ */
    [Inject] protected IApiService ApiService { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected ActivityState ActivityState { get; set; } = default!;
    [Inject] protected IDialogService DialogService { get; set; } = default!;

    /* ------------ داده‌ها و وضعیت ------------ */
    protected List<ActivityDto> _records = new();
    protected bool _loading = true;

    /* تایمر ثبت لحظه‌ای */
    protected int _seconds = 0;
    protected bool _isRunning = false;
    protected string _recordTitle = "";
    private System.Threading.Timer? _timer;

    /* ویرایش */
    private int _editId;
    private string _editTitle = "";
    private string _editTime = "00:00:00";
    private bool _editShowTitleError;

    /* حذف */
    private int _deleteId;

    /* ---------- ستون‌های جدول ---------- */
    protected List<DynamicTable<ActivityDto>.TableColumn> RecordColumns =>
        new()
        {
            new("title",        "عنوان رکورد",  r => b => b.AddContent(0, r.Title)),
            new("totalSeconds", "مدت زمان",     r => b => b.AddContent(0, FormatTime(r.TotalSeconds))),
            new("actions",      "عملیات",        r => b =>
            {
                b.OpenElement(0, "div");
                b.AddAttribute(1, "class", "actions-wrap");

                /* ✏️ Edit */
                b.OpenComponent<MudIconButton>(2);
                b.AddAttribute(3,"Icon",  Icons.Material.Filled.Edit);
                b.AddAttribute(4,"Color", Color.Info);
                b.AddAttribute(5,"Size",  Size.Small);
                b.AddAttribute(6,"Title","ویرایش");
                b.AddAttribute(7,"OnClick",
                    EventCallback.Factory.Create<MouseEventArgs>(this, _ => OpenEditModal(r)));
                b.CloseComponent();

                /* 🗑 Delete */
                b.OpenComponent<MudIconButton>(8);
                b.AddAttribute(9,"Icon",  Icons.Material.Filled.Delete);
                b.AddAttribute(10,"Color", Color.Error);
                b.AddAttribute(11,"Size", Size.Small);
                b.AddAttribute(12,"Title","حذف");
                b.AddAttribute(13,"OnClick",
                    EventCallback.Factory.Create<MouseEventArgs>(this, _ => OpenDeleteModal(r.Id)));
                b.CloseComponent();

                b.CloseElement();
            })
        };

    /* ---------- چرخه حیات ---------- */
    protected override async Task OnInitializedAsync()
    {
        await EnsureActivitySelected();
        await LoadRecordsAndUpdateTotal();
        _loading = false;
    }

    private async Task EnsureActivitySelected()
    {
        if (ActivityState.SelectedActivity is null)
        {
            var all = await ApiService.GetActivitiesAsync();
            var found = all.FirstOrDefault(a => a.Id == id);
            if (found != null)
                ActivityState.SetActivity(found);
        }
    }

    /* ---------- بارگذاری رکوردها و آپدیت مجموع ---------- */
    private async Task LoadRecordsAndUpdateTotal()
    {
        var data = await ApiService.GetTimeRecordsByActivityAsync(id);

        _records = data
            .OrderByDescending(r => r.Id)
            .Select(r => new ActivityDto
            {
                Id = r.Id,
                Title = r.Title,
                TotalSeconds = ToSeconds(r.Duration)
            }).ToList();

        var sum = _records.Sum(r => r.TotalSeconds);

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

    /* ---------- تایمر ---------- */
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

    /* ---------- ایجاد رکورد ---------- */
    protected async Task SaveRecord()
    {
        if (string.IsNullOrWhiteSpace(_recordTitle))
        {
            AppNotification.Instance?.ShowError("عنوان رکورد الزامی است");
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

        var ok = await ApiService.CreateTimeRecordAsync(dto);
        AppNotification.Instance?.ShowSuccess(ok ? "رکورد ذخیره شد" : "خطا در ذخیره رکورد");

        if (ok)
        {
            _recordTitle = "";
            _seconds = 0;
            await LoadRecordsAndUpdateTotal();
        }
    }

    /* ---------- ویرایش ---------- */
    private void OpenEditModal(ActivityDto rec)
    {
        _editId = rec.Id;
        _editTitle = rec.Title;
        _editTime = FormatTime(rec.TotalSeconds);
        _editShowTitleError = false;

        var prm = new DialogParameters
        {
            { "Title", "ویرایش رکورد" },
            { "OkText", "ذخیره" },
            { "ChildContent", (RenderFragment)(b =>
            {
                b.OpenElement(0,"div"); b.AddAttribute(1,"class","input-label"); b.AddContent(2,"عنوان"); b.CloseElement();
                b.OpenComponent<MudTextField<string>>(3);
                b.AddAttribute(4,"Value", _editTitle);
                b.AddAttribute(5,"ValueChanged", EventCallback.Factory.Create<string>(this, v => _editTitle = v));
                b.AddAttribute(6,"Immediate", true);
                b.AddAttribute(7,"Class", "modern-input");
                b.AddAttribute(8,"Variant", Variant.Outlined);
                b.AddAttribute(9,"Error", _editShowTitleError);
                b.AddAttribute(10,"ErrorText","عنوان الزامی است");
                b.CloseComponent();

                b.OpenElement(11,"div"); b.AddAttribute(12,"class","input-label mt-4"); b.AddContent(13,"مدت زمان (hh:mm:ss)"); b.CloseElement();
                b.OpenComponent<MudTextField<string>>(14);
                b.AddAttribute(15,"Value", _editTime);
                b.AddAttribute(16,"ValueChanged", EventCallback.Factory.Create<string>(this, v => _editTime = v));
                b.AddAttribute(17,"Immediate", true);
                b.AddAttribute(18,"Class", "modern-input");
                b.AddAttribute(19,"Placeholder","00:00:00");
                b.AddAttribute(20,"Variant", Variant.Outlined);
                b.CloseComponent();
            })},
            { "OnOk", EventCallback.Factory.Create(this, SaveEdit) }
        };

        DialogService.Show<AllModal>("ویرایش رکورد", prm,
            new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true });
    }

    private async Task SaveEdit()
    {
        _editShowTitleError = string.IsNullOrWhiteSpace(_editTitle);

        if (_editShowTitleError || !TimeSpan.TryParse(_editTime, out var ts))
        {
            AppNotification.Instance?.ShowError("ورودی‌ها معتبر نیستند");
            return;
        }

        var dto = new TimeRecordDto
        {
            Id = _editId,
            ActivityId = id,
            Title = _editTitle.Trim(),
            CheckIn = DateTime.Now.AddSeconds(-ts.TotalSeconds),
            CheckOut = DateTime.Now,
            Duration = _editTime
        };

        var ok = await ApiService.UpdateTimeRecordAsync(dto.Id, dto);
        AppNotification.Instance?.ShowSuccess(ok ? "ویرایش انجام شد" : "خطا در ویرایش");

        if (ok)
            await LoadRecordsAndUpdateTotal();
    }

    /* ---------- حذف ---------- */
    private void OpenDeleteModal(int recordId)
    {
        _deleteId = recordId;

        var prm = new DialogParameters
        {
            { "Title", "تأیید حذف رکورد" },
            { "OkText", "حذف" },
            { "CancelText","لغو" },
            { "ChildContent", (RenderFragment)(b =>
            {
                b.OpenElement(0,"div"); b.AddAttribute(1,"class","center-text");
                b.AddContent(2,"آیا مطمئن هستید؟"); b.CloseElement();
            })},
            { "OnOk", EventCallback.Factory.Create(this, DeleteRecord) }
        };

        DialogService.Show<AllModal>("حذف رکورد", prm,
            new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true });
    }

    private async Task DeleteRecord()
    {
        var ok = await ApiService.DeleteTimeRecordAsync(_deleteId);
        AppNotification.Instance?.ShowSuccess(ok ? "حذف شد" : "خطا در حذف");
        if (ok) await LoadRecordsAndUpdateTotal();
    }

    /* ---------- ناوبری ---------- */
    protected void GoBack() => NavManager.NavigateTo("/TaskLog");

    /* ---------- Utils ---------- */
    protected static string FormatTime(int s) =>
        TimeSpan.FromSeconds(s).ToString(@"hh\:mm\:ss");

    protected static int ToSeconds(string str) =>
        TimeSpan.TryParse(str, out var ts) ? (int)ts.TotalSeconds : 0;

    public void Dispose() => _timer?.Dispose();
}
