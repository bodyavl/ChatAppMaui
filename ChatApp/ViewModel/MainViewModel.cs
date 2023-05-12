using ChatApp.Model;
using ChatApp.Services;
using ChatApp.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    public partial class MainViewModel : BaseViewModel
    {
        [ObservableProperty]
        string messages;

        [ObservableProperty]
        string myMessage;

        public ObservableCollection<User> Users { get; private set; } = new();

        ClientService clientService;
        ApiService apiService;
        public MainViewModel(ClientService clientService, ApiService apiService)
        {
            this.clientService = clientService;
            this.apiService = apiService;
        }

        [RelayCommand]
        async Task CheckLogIn() 
        {
            var token = await SecureStorage.Default.GetAsync(nameof(LocalUser.AccessToken));
            if (token == null)
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
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
                    Users.Add(user);
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            finally { IsBusy = false; }
            
        }

        [RelayCommand]
        Task InitializeCheckLogIn() => CheckLogIn();

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
            await Shell.Current.GoToAsync(nameof(ChatPage), new Dictionary<string, object>
            {
                ["To"] = user,
            });
        }


    }
}
