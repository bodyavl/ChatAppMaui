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
        public Func<Task> RefreshChats;
        public Action<string, Message> ShowNewMessage;
        public ClientService() 
        {
            Task.Run(() => ConnectClient());
        }

        public async Task ConnectClient()
        {
            client = new SocketIO("https://node-chat-app-egyw.onrender.com", new SocketIOOptions
            {
                Auth = new { username = Task.Run(async () => await SecureStorage.Default.GetAsync(nameof(LocalUser.Username))).Result, 
                                userId = Task.Run(async () => await SecureStorage.Default.GetAsync(nameof(LocalUser._Id))).Result
                },
                ReconnectionAttempts = 120
            });

            client.On("receive message", (response) =>
            {
                
                var from = response.GetValue<string>();
                var content = response.GetValue<string>(1);
                Message message = new()
                {
                    FromSelf = false,
                    Content = content,
                    UpdatedAt = DateTime.Now,
                };


                ShowNewMessage(from, message);
                AddPrivateMessage?.Invoke(message);
            });
                
                

            await client.ConnectAsync();

        }
        public bool IsConnected()
        {
            return client.Connected;
        }

        public async Task DisconnectClient()
        {
            await client.DisconnectAsync();
        }


        public async Task SendPrivateMessageAsync(string content, string from, string to)
        {
            await RefreshChats();
            await client.EmitAsync("private message", new { content, from, to });
        }
    }
}
