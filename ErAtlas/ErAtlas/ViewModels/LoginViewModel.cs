using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isErrorVisible;

    public static event Action? OnUserLoggedOut;
    public static event Action<Utilisateur?>? OnUserLoggedIn;

    // Propriétés statiques pour gérer la session
    public static Utilisateur? CurrentUser { get; private set; }
    public static bool IsLoggedIn { get; private set; }
    public static bool IsGestionnaire { get; private set; }

    public LoginViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    private void Login()
    {
        try
        {
            var trimmedUsername = Username.Trim();

            if (string.IsNullOrWhiteSpace(trimmedUsername) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Veuillez remplir tous les champs.";
                IsErrorVisible = true;
                return;
            }

            // Vérifie le mot de passe et récupère l'utilisateur
            var user = _databaseService.VerifUser(trimmedUsername);
            if (user != null)
            {
                // Hash le mot de passe saisi pour comparaison
                string hashedPassword = _databaseService.HashMotDePasse(Password);
                if (user.MotDePasse == hashedPassword)
                {
                    // Récupère toutes les informations de l'utilisateur
                    var fullUser = _databaseService.LireUtilisateurs().FirstOrDefault(u => u.Id == user.Id);
                    if (fullUser != null)
                    {
                        IsErrorVisible = false;
                        CurrentUser = fullUser; // Stocke l'utilisateur connecté
                        IsLoggedIn = true; // Marque la session comme active
                        IsGestionnaire = fullUser.Gestionnaire; // Stocke le rôle de l'utilisateur

                        var app = Application.Current;
                        if (app?.Windows.Count > 0)
                        {
                            app.Windows[0].Page = new AppShell();
                        }

                        OnUserLoggedIn?.Invoke(fullUser);
                        return;
                    }
                }
            }

            ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
            IsErrorVisible = true;
        }
        catch (Exception)
        {
            ErrorMessage = "Connexion impossible. Verifiez que la base de donnees est disponible.";
            IsErrorVisible = true;
        }
    }

    // Méthode pour se déconnecter
    public static void Logout()
    {
        CurrentUser = null;
        IsLoggedIn = false;
        IsGestionnaire = false;
        OnUserLoggedOut?.Invoke();
    }
}