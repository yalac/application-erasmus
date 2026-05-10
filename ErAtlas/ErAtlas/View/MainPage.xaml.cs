using ErAtlas.ViewModels;
using Microsoft.Maui.Controls;

namespace ErAtlas.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainPageViewModel viewModel)
            {
                viewModel.RefreshTrajets();
            }
        }
    }
}