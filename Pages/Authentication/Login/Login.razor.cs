using BlazorApp1.Models;
using BlazorApp1.Services;
using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BlazorApp1.Pages.Authentication
{
    public partial class LoginBase : ComponentBase
    {
        [Inject] protected IApiService ApiService { get; set; }
        [Inject] protected IAuthService AuthService { get; set; }
        [Inject] protected NavigationManager Navigation { get; set; }
        [Inject] protected ISnackbar Snackbar { get; set; }
        [Inject] protected AuthenticationStateProvider AuthProvider { get; set; }

        protected readonly LoginDto _model = new();
        protected bool _showPassword;
        protected bool _busy;

        protected InputType _passwordInputType => _showPassword ? InputType.Text : InputType.Password;

        protected string _passwordToggleIcon => _showPassword
            ? Icons.Material.Filled.VisibilityOff
            : Icons.Material.Filled.Visibility;

        protected void TogglePassword() => _showPassword = !_showPassword;

        protected async Task HandleSubmit()
        {
            if (_busy) return;
            _busy = true;

            try
            {
                var result = await ApiService.LoginAsync(_model);
                if (result is null || string.IsNullOrWhiteSpace(result.Token))
                {
                    Snackbar.Add("Login failed: invalid credentials", Severity.Error);
                    return;
                }

                var user = new UserDto
                {
                    Id = result.UserId,
                    UserName = result.UserName,
                    FullName = result.FullName,
                    Role = result.Role
                };

                await AuthService.LoginAsync(result.Token, user);

                if (AuthProvider is CustomAuthStateProvider p)
                    p.NotifyUserChanged();

                Snackbar.Add("Login successful!", Severity.Success);
                Navigation.NavigateTo("/home", true);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Login error: {ex.Message}", Severity.Error);
            }
            finally
            {
                _busy = false;
            }
        }
    }
}
