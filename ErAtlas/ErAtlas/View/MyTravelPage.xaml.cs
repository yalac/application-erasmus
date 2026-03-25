using ErAtlas.ViewModels;

namespace ErAtlas.View;

public partial class MyTravelPage
{
    public MyTravelPage(MyTravelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}