using ErAtlas.ViewModels;
namespace ErAtlas.View;

public partial class TripManagementPage : ContentPage
{
    public TripManagementPage(TripManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}