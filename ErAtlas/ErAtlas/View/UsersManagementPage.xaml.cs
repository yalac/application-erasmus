using ErAtlas.ViewModels;
using ErAtlas.Model;

namespace ErAtlas.View;

public partial class UsersManagementPage
{
    public UsersManagementPage(UsersManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // La vérification d'accès se fera via les propriétés calculées du ViewModel
    }

    private async void OnNouveauUtilisateurClicked(object sender, EventArgs e)
    {
        var viewModel = (UsersManagementViewModel)BindingContext;

        viewModel.AffichageFormulaireDeCreationCommand.Execute(null);

        await Navigation.PushModalAsync(new UsersManagementPopupPage(viewModel));
    }

    private async void OnModifierClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is not Utilisateur utilisateur)
        {
            return;
        }

        var viewModel = (UsersManagementViewModel)BindingContext;

        viewModel.ModificationUtilisateurCommand.Execute(utilisateur);
        await Navigation.PushModalAsync(new UsersManagementPopupPageEdit(viewModel));
    }
}