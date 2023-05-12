using ChatApp.Services;
using ChatApp.View;
using ChatApp.ViewModel;
using Microsoft.Extensions.Logging;
using SocketIOClient;
using CommunityToolkit.Maui;

namespace ChatApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
            .UseMauiCommunityToolkit();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SignUpPage>();
		builder.Services.AddTransient<SearchPage>();
		builder.Services.AddTransient<ChatPage>();

        builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<SignUpViewModel>();
        builder.Services.AddTransient<SearchViewModel>();
		builder.Services.AddTransient<ChatViewModel>();

        builder.Services.AddSingleton<ClientService>();
        builder.Services.AddSingleton<ApiService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
