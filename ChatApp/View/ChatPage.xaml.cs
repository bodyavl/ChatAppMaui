using ChatApp.ViewModel;

namespace ChatApp.View;

public partial class ChatPage : ContentPage
{
	public ChatPage(ChatViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();

	}
	
}