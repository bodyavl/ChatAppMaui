
using ChatApp.ViewModel;
using Microsoft.Maui.Controls.PlatformConfiguration;
using SocketIOClient;
using System.Runtime.CompilerServices;

namespace ChatApp;

public partial class MainPage : ContentPage
{

	public MainPage(MainViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
    }

}

