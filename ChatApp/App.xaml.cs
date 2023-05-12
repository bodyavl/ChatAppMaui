namespace ChatApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		MainPage = new AppShell();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window =  base.CreateWindow(activationState);

		window.Width = 400;
		window.Height = 700;
		window.MinimumHeight = 400;
		window.MinimumWidth = 300;

		window.X = 600;
		window.Y = 80;
		return window;
    }
}
