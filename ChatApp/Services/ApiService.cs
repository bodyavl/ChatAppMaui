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

            var user = await response.Content.ReadFromJsonAsync<LocalUser>();

            if (user != null)
            {
                await SecureStorage.Default.SetAsync(nameof(LocalUser._Id), user._Id);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.AccessToken), user.AccessToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.RefreshToken), user.RefreshToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.Username), user.Username);
                if (!clientService.IsConnected()) await clientService.ConnectClient();
            }

            return user;

        }
        public async Task<LocalUser> SignUpAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/user/signup", new { username = username, password = password });

            var user = await response.Content.ReadFromJsonAsync<LocalUser>();

            if (user != null)
            {
                await SecureStorage.Default.SetAsync(nameof(LocalUser._Id), user._Id);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.AccessToken), user.AccessToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.RefreshToken), user.RefreshToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.Username), user.Username);
                if (!clientService.IsConnected()) await clientService.ConnectClient();
            }

            return user;

        }

        public async Task<List<User>> GetUsersAsync(string query) 
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/user/search?q={query}");
            var token = await SecureStorage.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");
            
            var response = await _httpClient.SendAsync(request);

            var users = await response.Content.ReadFromJsonAsync<List<User>>();

            return users;
        }

        public async Task<List<User>> GetChatsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/user/chats");
            var token = await SecureStorage.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            var users = await response.Content.ReadFromJsonAsync<List<User>>();

            return users;
        }

        public async Task LogoutAsync()
        {
            var token = await SecureStorage.GetAsync(nameof(LocalUser.RefreshToken));
            var response = await _httpClient.PostAsJsonAsync("/user/logout", new { refreshToken = token });
            
            SecureStorage.Default.Remove(nameof(LocalUser.RefreshToken));
            SecureStorage.Default.Remove(nameof(LocalUser.AccessToken));
            SecureStorage.Default.Remove(nameof(LocalUser.Username));

            if (clientService.IsConnected()) await clientService.DisconnectClient();
        }

        public async Task AddMessageAsync(string from, string to, string content)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/message/add");
            var token = await SecureStorage.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Method = HttpMethod.Post;
            request.Content = JsonContent.Create(new { content, from, to });

            var response = await _httpClient.SendAsync(request);

        }

        public async Task<List<Message>> GetMessagesAsync(string from, string to)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/message?from={from}&to={to}");
            var token = await SecureStorage.GetAsync(nameof(LocalUser.AccessToken));
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            var messages = await response.Content.ReadFromJsonAsync<List<Message>>();

            return messages;
        }



    }
}
