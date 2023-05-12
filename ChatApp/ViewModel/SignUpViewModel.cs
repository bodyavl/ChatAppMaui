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
        public SignUpViewModel(ApiService apiService)
        {
            this.apiService = apiService;
        }

        [RelayCommand]
        async Task SignUp()
        {
            try
            {
                var user = await apiService.SignUpAsync(Username, Password);
                if (user != null) await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            

        }

    }
}
