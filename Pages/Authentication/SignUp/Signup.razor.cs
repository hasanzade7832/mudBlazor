using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazored.Toast.Services;
using BlazorApp1.Models.Auth;
using BlazorApp1.Services.Interfaces;

public class SignupBase : ComponentBase
{
    [Inject] public IApiService ApiService { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    [Inject] public IToastService ToastService { get; set; }

    protected RegisterDto Model { get; set; } = new();
    protected bool ShowPassword { get; set; }
    protected bool IsLoading { get; set; }

    protected void TogglePassword() => ShowPassword = !ShowPassword;

    protected async Task HandleSubmit()
    {
        IsLoading = true;

        try
        {
            var result = await ApiService.RegisterAsync(Model);

            if (string.IsNullOrWhiteSpace(result) ||
                result.Contains("خطا") ||
                result.Contains("رمز عبور") ||
                result.Contains("قبلاً") ||
                result.Contains("کوتاه") ||
                result.Length < 10)
            {
                ToastService.ShowError(result.Trim('"'));
                return;
            }

            ToastService.ShowSuccess(result.Trim('"'));
            await Task.Delay(1500);
            Navigation.NavigateTo("/login", true); // پاکسازی توست‌ها
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"خطای سیستم: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected void BackToLogin()
    {
        Navigation.NavigateTo("/login");
    }
}
