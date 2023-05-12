using ChatApp.Services;
using ChatApp.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;

        ApiService apiService;
        public LoginViewModel(ApiService apiService) 
        {
            this.apiService = apiService;
        }

        [RelayCommand]
        async Task Login()
        {
            try
            {
                IsBusy = true;
                var user = await apiService.LoginAsync(Username, Password);
                if (user != null) await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            finally { IsBusy = false; }
            
            
        }

        [RelayCommand]
        async Task GoToSignUp() => await Shell.Current.GoToAsync(nameof(SignUpPage));
    }
}
