using ChatApp.ViewModel;

namespace ChatApp.View;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}