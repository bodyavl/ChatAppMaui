using ChatApp.ViewModel;

namespace ChatApp.View;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(SignUpViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}