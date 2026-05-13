using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using ErAtlas.Database;

namespace ErAtlas.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    [ObservableProperty]
    private string _username;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private string _errorMessage;
    [ObservableProperty]
    private bool _isErrorVisible;



    public LoginViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    [RelayCommand]
    private void Login()
    {
        var trimmedUsername = Username.Trim();

        if (string.IsNullOrWhiteSpace(trimmedUsername) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez remplir tous les champs.";
            IsErrorVisible = true;
            return;
        }

        var isValidUser = _databaseService.CheckUser(trimmedUsername, Password);

        if (isValidUser)
        {
            IsErrorVisible = false;

            // Récupère AppShell du conteneur DI pour que les dépendances soient disponibles
            var appShell = Application.Current.Handler.MauiContext?.Services.GetService<AppShell>();
            if (appShell != null)
            {
                Application.Current.MainPage = appShell;
            }

            return;
        }

        ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
        IsErrorVisible = true;
    }
}

