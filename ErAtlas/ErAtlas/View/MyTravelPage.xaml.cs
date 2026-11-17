using ErAtlas.ViewModels;

namespace ErAtlas.View;

public partial class MyTravelPage
{
    public MyTravelPage(MyTravelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MyTravelViewModel viewModel)
        {
            viewModel.RefreshTravels();
        }
    }
}