using ChatApp.ViewModel;

namespace ChatApp.View;

public partial class SearchPage : ContentPage
{
	public SearchPage(SearchViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		searchBar.Focus();
    }
}