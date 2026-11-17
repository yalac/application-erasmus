using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _nom = string.Empty;
    [ObservableProperty]
    private string _prenom = string.Empty;
    [ObservableProperty]
    private string _email = string.Empty;
    [ObservableProperty]
    private string _adresse = string.Empty;
    [ObservableProperty]
    private int _codePostal;
    [ObservableProperty]
    private string _ville = string.Empty;

    public SettingsViewModel()
    {
        LoadUserData();

        LoginViewModel.OnUserLoggedIn += _ => LoadUserData();
        LoginViewModel.OnUserLoggedOut += ClearUserData;
    }

    private void LoadUserData()
    {
        if (LoginViewModel.IsLoggedIn && LoginViewModel.CurrentUser != null)
        {
            var user = LoginViewModel.CurrentUser;
            Nom = user.Nom ?? "Non spécifié";
            Prenom = user.Prenom ?? "Non spécifié";
            Email = user.Email ?? "Non spécifié";
            Adresse = user.Adresse ?? "Non spécifiée";
            CodePostal = user.CodePostal;
            Ville = user.Ville ?? "Non spécifiée";
        }
    }

    private void ClearUserData()
    {
        Nom = string.Empty;
        Prenom = string.Empty;
        Email = string.Empty;
        Adresse = string.Empty;
        CodePostal = 0;
        Ville = string.Empty;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        LoginViewModel.Logout(); // Déconnecte l'utilisateur

        // Redirige vers la page de connexion
        await Shell.Current.GoToAsync("//LoginPage");
    }
}