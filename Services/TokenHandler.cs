using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp1.Services
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _nav;
        private readonly AuthenticationStateProvider _authProvider;

        public TokenHandler(
            ILocalStorageService localStorage,
            NavigationManager nav,
            AuthenticationStateProvider authProvider)
        {
            _localStorage = localStorage;
            _nav = nav;
            _authProvider = authProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _localStorage.RemoveItemAsync("token");
                await _localStorage.RemoveItemAsync("user");

                if (_authProvider is CustomAuthStateProvider stateProvider)
                    stateProvider.NotifyUserChanged(); 

                _nav.NavigateTo("/login", forceLoad: true);
            }

            return response;
        }
    }
}
