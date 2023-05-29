using ChatApp.Model;
using ChatApp.Services;
using ChatApp.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    public partial class MainViewModel : BaseViewModel
    {

        public ObservableCollection<User> Users { get; private set; } = new();

        ClientService clientService;
        ApiService apiService;
        public MainViewModel(ClientService clientService, ApiService apiService)
        {
            this.clientService = clientService;
            this.apiService = apiService;

            clientService.RefreshChats = GetChats;
            clientService.ShowNewMessage = (string from, Message message) =>
            {
                var foundSender = Users.FirstOrDefault(user => user._Id == from);
                var sender = new User
                {
                    _Id = foundSender._Id,
                    Username = foundSender.Username,
                    LastMessage = message,
                    LastMessageTime = message.UpdatedAt.ToString("HH:mm")
                };
                Users.Remove(foundSender);
                if (!sender.IsSentMessage)
                {
                    sender.IsSentMessage = true;
                }

                var request = new NotificationRequest
                {
                    Title = sender.Username,
                    Description = sender.LastMessage.Content,
                };
#if ANDROID
                LocalNotificationCenter.Current.Show(request);
#endif

                Users.Insert(0, sender);
                
            };
        }

        [RelayCommand]
        async Task GetChats()
        { 
            try
            {
                IsBusy = true;
                var users = await apiService.GetChatsAsync();
                if (users == null) return;

                if (Users.Count > 0) Users.Clear();
                foreach (var user in users)
                {
                    var updatedAt = user.LastMessage.UpdatedAt.ToLocalTime();
                    var now = DateTime.Now;
                    var comparison = now - updatedAt;
                    if(comparison.TotalHours < 24)
                    {
                        user.LastMessageTime = updatedAt.ToString("HH:mm");
                    }
                    else if(comparison.TotalHours < 144)
                    {
                        user.LastMessageTime = updatedAt.DayOfWeek.ToString();
                    }
                    else
                    {
                        user.LastMessageTime = updatedAt.ToString("d");
                    }
                    Users.Add(user);
                }
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized || e.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    await GoToLogin();
                }
                else if (e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    await Shell.Current.DisplayAlert("Error occured", "Internal server error", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            finally { IsBusy = false; }

            
        }

        [RelayCommand]

        async Task GoToLogin() => await Shell.Current.GoToAsync(nameof(LoginPage));

        [RelayCommand]
        async Task GoToSearch() => await Shell.Current.GoToAsync(nameof(SearchPage));

        [RelayCommand]
        async Task Logout()
        {
            try
            {
                await apiService.LogoutAsync();
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
        }

        [RelayCommand]
        async Task GoToChat(User user)
        {
            Users[Users.IndexOf(user)].IsSentMessage = false;

            await Shell.Current.GoToAsync(nameof(ChatPage), new Dictionary<string, object>
            {
                ["To"] = user,
            });
        }


    }
}
