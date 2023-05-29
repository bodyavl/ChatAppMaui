using ChatApp.Model;
using ChatApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    public partial class SignUpViewModel : BaseViewModel
    {
        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;

        ApiService apiService;
        ClientService clientService;
        public SignUpViewModel(ApiService apiService, ClientService clientService)
        {
            this.apiService = apiService;
            this.clientService = clientService;
        }

        [RelayCommand]
        async Task SignUp()
        {
            try
            {
                var user = await apiService.SignUpAsync(Username, Password);
                if (user != null) return;

                await SecureStorage.Default.SetAsync(nameof(LocalUser._Id), user._Id);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.AccessToken), user.AccessToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.RefreshToken), user.RefreshToken);
                await SecureStorage.Default.SetAsync(nameof(LocalUser.Username), user.Username);
                if (!clientService.IsConnected()) await clientService.ConnectClient();

                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch(HttpRequestException e)
            {
                if(e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    await Shell.Current.DisplayAlert("Error occured", "User whith current username doesn't exist", "OK");
                }
                
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            

        }

    }
}
