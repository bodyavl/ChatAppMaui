using ChatApp.Model;
using ChatApp.Services;
using ChatApp.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    public partial class SearchViewModel : BaseViewModel
    {
        [ObservableProperty]
        string query;
        [ObservableProperty]
        bool isbusy;
        public ObservableCollection<ChatUser> Users { get; private set; } = new();

        ApiService apiService;
        public SearchViewModel(ApiService apiService)
        {
            this.apiService = apiService;
        }

        [RelayCommand]
        async Task PerformSearch(string query)
        {
            try
            {
                IsBusy = true;
                var users = await apiService.GetUsersAsync(query);
                if (Users.Count > 0) Users.Clear();
                foreach (ChatUser user in users)
                {
                    Users.Add(user);
                }
                IsBusy = false;
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            
        }

        [RelayCommand]
        async Task GoToChat(ChatUser user)
        {
            await Shell.Current.GoToAsync(nameof(ChatPage), new Dictionary<string, object>
            {
                ["To"] = user,
            });
        }
    }
}
