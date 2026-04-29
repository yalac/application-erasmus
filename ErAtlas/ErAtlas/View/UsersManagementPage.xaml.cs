using ErAtlas.ViewModels;
using ErAtlas.Model;

namespace ErAtlas.View;

public partial class UsersManagementPage : ContentPage
{
    public UsersManagementPage(UsersManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnNouveauUtilisateurClicked(object sender, EventArgs e)
    {
        var viewModel = (UsersManagementViewModel)BindingContext;

        await Navigation.PushModalAsync(new UsersManagementPopupPage(viewModel));
    }

    private void OnSupprimerClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is Utilisateur utilisateur)
        {
            var viewModel = (UsersManagementViewModel)BindingContext;
            viewModel.SupprimerUtilisateurCommand.Execute(utilisateur);
        }
    }
}