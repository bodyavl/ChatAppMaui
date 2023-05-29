using ChatApp.Model;
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
        ClientService clientService;
        public LoginViewModel(ApiService apiService, ClientService clientService) 
        {
            this.apiService = apiService;
            this.clientService = clientService;
        }

        [RelayCommand]
        async Task Login()
        {
            try
            {
                IsBusy = true;
                var user = await apiService.LoginAsync(Username, Password);
                if (user == null) { return; }
                
                await SecureStorage.Default.SetAsync(nameof(LocalUser._Id), user._Id);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.AccessToken), user.AccessToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.RefreshToken), user.RefreshToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.Username), user.Username);
                if (!clientService.IsConnected()) await clientService.ConnectClient();

                if (user != null) await GoToMain();
            }
            catch(HttpRequestException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    await Shell.Current.DisplayAlert("Error occured", "Username and password doesn't match", "OK");
                }
                else if(ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    await Shell.Current.DisplayAlert("Error occured", "User doesn't exist", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error occured", ex.Message, "OK");
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            finally { IsBusy = false; }
            
            
        }
        async Task GoToMain() => await Shell.Current.GoToAsync($"//{nameof(MainPage)}");

        [RelayCommand]
        async Task GoToSignUp() => await Shell.Current.GoToAsync(nameof(SignUpPage));
    }
}
