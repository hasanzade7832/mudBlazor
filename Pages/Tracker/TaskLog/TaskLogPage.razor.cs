using BlazorApp1.Components;
using BlazorApp1.Components.DynamicTable;
using BlazorApp1.Models;
using BlazorApp1.Services.Interfaces;
using BlazorApp1.Services.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlazorApp1.Components.Modal;

public class TaskLogPageBase : ComponentBase
{
    [Inject] protected IApiService ApiService { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected ActivityState ActivityState { get; set; } = default!;
    [Inject] protected IDialogService DialogService { get; set; } = default!;

    protected List<ActivityDto> activities = new();
    protected string newTitle = "";
    protected string newTime = "00:00:00";
    protected bool showTitleError;
    protected bool loading = true;

    /* -------------------------------- ستون‌های جدول -------------------------------- */
    /* ستون actions (با آیکن چشم) */
    protected List<DynamicTable<ActivityDto>.TableColumn> ActivityColumns =>
        new()
        {
        new("title",        "عنوان",      a => b => b.AddContent(0, a.Title)),
        new("totalSeconds", "مدت زمان",  a => b => b.AddContent(0, FormatTime(a.TotalSeconds))),
        new("actions",      "عملیات",     a => b =>
        {
            b.OpenElement(0, "div");
            b.AddAttribute(1, "class", "actions-wrap");   // flex‑center

            /* 👁 آیکن جزئیات */
            b.OpenComponent<MudIconButton>(2);
            b.AddAttribute(3, "Icon", Icons.Material.Filled.RemoveRedEye);  // چشم
            b.AddAttribute(4, "Color", Color.Primary);
            b.AddAttribute(5, "Size",  Size.Small);
            b.AddAttribute(6, "Title", "جزئیات");
            b.AddAttribute(7, "OnClick",
                EventCallback.Factory.Create<MouseEventArgs>(this, _ => NavigateToDetails(a.Id)));
            b.CloseComponent();

            /* ✏️ آیکن ویرایش */
            b.OpenComponent<MudIconButton>(8);
            b.AddAttribute(9, "Icon",  Icons.Material.Filled.Edit);
            b.AddAttribute(10,"Color", Color.Info);
            b.AddAttribute(11,"Size",  Size.Small);
            b.AddAttribute(12,"Title", "ویرایش");
            b.AddAttribute(13,"OnClick",
                EventCallback.Factory.Create<MouseEventArgs>(this, _ => OpenEditModal(a)));
            b.CloseComponent();

            /* 🗑 آیکن حذف */
            b.OpenComponent<MudIconButton>(14);
            b.AddAttribute(15,"Icon",  Icons.Material.Filled.Delete);
            b.AddAttribute(16,"Color", Color.Error);
            b.AddAttribute(17,"Size",  Size.Small);
            b.AddAttribute(18,"Title", "حذف");
            b.AddAttribute(19,"OnClick",
                EventCallback.Factory.Create<MouseEventArgs>(this, _ => OpenDelete(a.Id)));
            b.CloseComponent();

            b.CloseElement();
        }),
        };


    /* -------------------------------- LifeCycle -------------------------------- */
    protected override async Task OnInitializedAsync() => await LoadActivities();

    private async Task LoadActivities()
    {
        loading = true;
        try { activities = await ApiService.GetActivitiesAsync(); }
        catch { AppNotification.Instance?.ShowError("خطا در دریافت لیست فعالیت‌ها"); }
        loading = false;
    }

    /* -------------------------------- CRUD: افزودن -------------------------------- */
    protected async Task AddActivity()
    {
        showTitleError = string.IsNullOrWhiteSpace(newTitle);
        if (showTitleError) { AppNotification.Instance?.ShowError("عنوان را وارد کنید"); return; }

        if (!TimeSpan.TryParse(newTime, out var ts))
        { AppNotification.Instance?.ShowError("زمان نامعتبر است"); return; }

        var dto = new ActivityDto { Title = newTitle.Trim(), TotalSeconds = (int)ts.TotalSeconds };
        var ok = await ApiService.CreateActivityAsync(dto);

        AppNotification.Instance?.ShowSuccess(ok ? "فعالیت افزوده شد" : "خطا در افزودن");
        if (ok) await LoadActivities();
        newTitle = ""; newTime = "00:00:00"; showTitleError = false;
    }

    /* -------------------------------- جزئیات (NavigateToDetails) -------------------------------- */
    protected Task NavigateToDetails(int id)
    {
        var sel = activities.FirstOrDefault(a => a.Id == id);
        if (sel != null) ActivityState.SetActivity(sel);
        NavManager.NavigateTo($"/TaskLog/Details/{id}");
        return Task.CompletedTask;
    }

    /* -------------------------------- ویرایش -------------------------------- */
    private int editId;
    private string editTitle = "";
    private string editTime = "00:00:00";
    private bool editShowTitleError;

    private void OpenEditModal(ActivityDto a)
    {
        editId = a.Id; editTitle = a.Title; editTime = FormatTime(a.TotalSeconds); editShowTitleError = false;

        var prm = new DialogParameters
        {
            { "Title", "ویرایش فعالیت" },
            { "ChildContent", (RenderFragment)(b =>
            {
                b.OpenElement(0,"div"); b.AddAttribute(1,"class","input-label"); b.AddContent(2,"عنوان جدید"); b.CloseElement();
                b.OpenComponent<MudTextField<string>>(3);
                b.AddAttribute(4,"Class","modern-input");
                b.AddAttribute(5,"Variant",Variant.Outlined);
                b.AddAttribute(6,"Value",editTitle);
                b.AddAttribute(7,"ValueChanged", EventCallback.Factory.Create<string>(this,v=>editTitle=v));
                b.AddAttribute(8,"Immediate",true);
                b.AddAttribute(9,"Error", editShowTitleError);
                b.AddAttribute(10,"ErrorText","عنوان الزامی است");
                b.CloseComponent();

                b.OpenElement(11,"div"); b.AddAttribute(12,"class","input-label"); b.AddContent(13,"مدت زمان"); b.CloseElement();
                b.OpenComponent<MudTextField<string>>(14);
                b.AddAttribute(15,"Class","modern-input");
                b.AddAttribute(16,"Placeholder","00:00:00");
                b.AddAttribute(17,"Variant",Variant.Outlined);
                b.AddAttribute(18,"Value",editTime);
                b.AddAttribute(19,"ValueChanged", EventCallback.Factory.Create<string>(this,v=>editTime=v));
                b.AddAttribute(20,"Immediate",true);
                b.CloseComponent();
            })},
            { "OnOk", EventCallback.Factory.Create(this, SaveEdit) }
        };

        DialogService.Show<AllModal>("ویرایش فعالیت", prm,
            new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true });
    }

    private async Task SaveEdit()
    {
        if (!TimeSpan.TryParse(editTime, out var ts) || string.IsNullOrWhiteSpace(editTitle))
        { AppNotification.Instance?.ShowError("ورودی‌ها معتبر نیستند"); return; }

        var dto = new ActivityDto { Id = editId, Title = editTitle.Trim(), TotalSeconds = (int)ts.TotalSeconds };
        var ok = await ApiService.UpdateActivityAsync(editId, dto);
        AppNotification.Instance?.ShowSuccess(ok ? "ویرایش شد" : "خطا در ویرایش");
        if (ok) await LoadActivities();
    }

    /* -------------------------------- حذف -------------------------------- */
    private int deleteId;
    private void OpenDelete(int id)
    {
        deleteId = id;

        var prm = new DialogParameters
        {
            { "Title", "تأیید حذف" },
            { "ChildContent", (RenderFragment)(b =>
            {
                b.OpenElement(0,"div"); b.AddAttribute(1,"class","center-text");
                b.AddContent(2,"آیا مطمئن هستید که این فعالیت حذف شود؟"); b.CloseElement();
            })},
            { "OkText","حذف" }, { "CancelText","لغو" },
            { "OnOk", EventCallback.Factory.Create(this, DeleteActivity) }
        };

        DialogService.Show<AllModal>("تأیید حذف", prm,
            new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true });
    }

    private async Task DeleteActivity()
    {
        var ok = await ApiService.DeleteActivityAsync(deleteId);
        AppNotification.Instance?.ShowSuccess(ok ? "حذف شد" : "خطا در حذف");
        if (ok) await LoadActivities();
    }

    /* -------------------------------- Utils -------------------------------- */
    protected static string FormatTime(int s) =>
        TimeSpan.FromSeconds(s).ToString(@"hh\:mm\:ss");
}
