
using ErAtlas.ViewModels;

namespace ErAtlas.View;

public partial class MyTravelDetailsPage
{
    public MyTravelDetailsPage(MyTravelDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }
}
