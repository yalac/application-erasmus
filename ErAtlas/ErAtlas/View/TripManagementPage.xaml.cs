using ErAtlas.ViewModels;
namespace ErAtlas.View;

public partial class TripManagementPage : ContentPage
{
    public TripManagementPage(TripManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // La vérification d'accès se fera via les propriétés calculées du ViewModel
    }

    private async void OnModifierClicked(object sender, EventArgs e)
    {
        if (MainScroll is not null)
        {
            await MainScroll.ScrollToAsync(0, 0, true);
        }
    }
}