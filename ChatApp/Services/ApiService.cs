using ChatApp.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Services
{
    public class ApiService
    {
        HttpClient _httpClient;
        ClientService clientService;
        public ApiService(ClientService clientService)
        {
            this.clientService = clientService;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://node-chat-app-egyw.onrender.com");
        }

        public async Task<LocalUser> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/user/login", new { username, password });

            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<LocalUser>();

            return user;

        }

        public async Task<UserTokens> UpdateTokens(string refreshToken)
        {
            var response = await _httpClient.PostAsJsonAsync("/user/token", new { refreshToken });

            response.EnsureSuccessStatusCode();

            var tokens = await response.Content.ReadFromJsonAsync<UserTokens>();
            
            return tokens;

        }
        public async Task<LocalUser> SignUpAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/user/signup", new { username, password });

            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<LocalUser>();


            return user;

        }

        public async Task<List<User>> GetUsersAsync(string query) 
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/user/search?q={query}");
            var token = await SecureStorage.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");
            
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var refreshToken = await SecureStorage.Default.GetAsync(nameof(LocalUser.RefreshToken));
                var newTokens = await UpdateTokens(refreshToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.AccessToken), newTokens.AccessToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.RefreshToken), newTokens.RefreshToken);
                token = await SecureStorage.Default.GetAsync(nameof(LocalUser.AccessToken));
                request = new HttpRequestMessage(HttpMethod.Get, $"/user/search?q={query}");
                request.Headers.Add("Authorization", $"Bearer {token}");
                response = await _httpClient.SendAsync(request);
            }

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<User>>();

            return users;
        }

        public async Task<List<User>> GetChatsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/user/chats");
            var token = await SecureStorage.Default.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var refreshToken = await SecureStorage.Default.GetAsync(nameof(LocalUser.RefreshToken));
                var newTokens = await UpdateTokens(refreshToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.AccessToken), newTokens.AccessToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.RefreshToken), newTokens.RefreshToken);
                token = await SecureStorage.Default.GetAsync(nameof(LocalUser.AccessToken));
                request = new HttpRequestMessage(HttpMethod.Get, $"/user/chats");
                request.Headers.Add("Authorization", $"Bearer {token}");
                response = await _httpClient.SendAsync(request);
            }

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            


            return users;
        }

        public async Task LogoutAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/user/logout");

            var refreshToken = await SecureStorage.GetAsync(nameof(LocalUser.RefreshToken));
            request.Content = JsonContent.Create(new { refreshToken });

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            
            SecureStorage.Default.Remove(nameof(LocalUser.RefreshToken));
            SecureStorage.Default.Remove(nameof(LocalUser.AccessToken));
            SecureStorage.Default.Remove(nameof(LocalUser.Username));

            if (clientService.IsConnected()) await clientService.DisconnectClient();
        }

        public async Task AddMessageAsync(string from, string to, string content)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/message/add");
            var token = await SecureStorage.Default.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Method = HttpMethod.Post;
            request.Content = JsonContent.Create(new { content, from, to });

            var response = await _httpClient.SendAsync(request);

        }

        public async Task<List<Message>> GetMessagesAsync(string from, string to)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/message?from={from}&to={to}");
            var token = await SecureStorage.Default.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var refreshToken = await SecureStorage.Default.GetAsync(nameof(LocalUser.RefreshToken));
                var newTokens = await UpdateTokens(refreshToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.AccessToken), newTokens.AccessToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.RefreshToken), newTokens.RefreshToken);
                token = await SecureStorage.Default.GetAsync(nameof(LocalUser.AccessToken));
                request = new HttpRequestMessage(HttpMethod.Get, $"/message?from={from}&to={to}");
                request.Headers.Add("Authorization", $"Bearer {token}");
                response = await _httpClient.SendAsync(request);
            }

            response.EnsureSuccessStatusCode();

            var messages = await response.Content.ReadFromJsonAsync<List<Message>>();

            return messages;
        }



    }
}
