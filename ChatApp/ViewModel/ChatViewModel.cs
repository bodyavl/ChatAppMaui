using ChatApp.Model;
using ChatApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    [QueryProperty("To", "To")]
    public partial class ChatViewModel : BaseViewModel
    {
        [ObservableProperty]
        string content;
        [ObservableProperty]
        ChatUser to;
        public ObservableCollection<Message> Messages { get; private set; } = new();

        ClientService clientService;
        ApiService apiService;

        public ChatViewModel(ApiService apiService, ClientService clientService)
        {
            clientService.AddPrivateMessage = (message) =>
            {
                Messages.Add(message);
            };

            this.apiService = apiService;
            this.clientService = clientService;

        }
        [RelayCommand]
        async Task GetMessages()
        {
            try
            {
                IsBusy = true;
                var id = await SecureStorage.GetAsync(nameof(LocalUser._Id));
                var messages = await apiService.GetMessagesAsync(id, To._Id);

                if (Messages.Count > 0) Messages.Clear();
                if (messages == null) return;

                foreach (var message in messages)
                {
                    Messages.Add(message);
                }

                var fakeMessage = new Message();
                await Task.Delay(0).ContinueWith(t => Messages.Add(fakeMessage));

                Messages.Remove(fakeMessage);

            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                
            }
        }

        [RelayCommand]
        async Task AddMessage()
        {
            try
            {
                var id = await SecureStorage.GetAsync(nameof(LocalUser._Id));
                await apiService.AddMessageAsync(id, To._Id, Content);
                await clientService.SendPrivateMessageAsync(Content, id, To._Id);
                Messages.Add(new Message { Content = Content, FromSelf = true });
                Content = "";
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }
        }
    }
}
