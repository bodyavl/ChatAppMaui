using ChatApp.Model;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Services
{
    public class ClientService
    {
        private SocketIO client;


        public Action<string> AddMessage;

        public Action<Message> AddPrivateMessage;
        public ClientService() 
        {
            Task.Run(ConnectClient);
        }

        public async Task ConnectClient()
        {
            try
            {
                client = new SocketIO("https://node-chat-app-egyw.onrender.com", new SocketIOOptions
                {
                    Auth = new { username = Task.Run(async () => await SecureStorage.Default.GetAsync(nameof(LocalUser.Username))).Result, 
                                 userId = Task.Run(async () => await SecureStorage.Default.GetAsync(nameof(LocalUser._Id))).Result
                    },
                });

                client.On("receive message", (response) =>
                {
                    var content = response.GetValue<string>();
                    
                    AddPrivateMessage(new Message { Content = content, FromSelf = false});
                });
                

                await client.ConnectAsync();

            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Error occured", e.Message, "OK");
            }

        }
        public bool IsConnected()
        {
            return client.Connected;
        }

        public async Task DisconnectClient()
        {
            await client.DisconnectAsync();
        }

        public async Task SendMessageAsync(string message)
        {
            await client.EmitAsync("chat message", message);
        }

        public async Task SendPrivateMessageAsync(string content, string to)
        {
            await client.EmitAsync("private message", new { content, to });
        }
    }
}
