using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp1.Models;

namespace BlazorApp1.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            var user = await _localStorage.GetItemAsync<UserDto>("user");

            if (string.IsNullOrWhiteSpace(token) || user is null)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName ?? "")
            }, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
