using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class UsersManagementViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<Utilisateur> _utilisateurs = new();

    [ObservableProperty]
    private string _nom = string.Empty;
    [ObservableProperty]
    private string _prenom = string.Empty;
    [ObservableProperty]
    private string _email = string.Empty;
    [ObservableProperty]
    private string _login = string.Empty;
    [ObservableProperty]
    private string _motDePasse = string.Empty;
    [ObservableProperty]
    private string _numeroTelephone = string.Empty;
    [ObservableProperty]
    private string _adresse = string.Empty;
    [ObservableProperty]
    private string _codePostal = string.Empty;
    [ObservableProperty]
    private string _ville = string.Empty;
    [ObservableProperty]
    private bool _gestionnaire; 

    [ObservableProperty]
    private string _errorMessage = string.Empty;
    [ObservableProperty]
    private bool _isErrorVisible;
    [ObservableProperty]
    private string _successMessage = string.Empty;
    [ObservableProperty]
    private bool _isSuccessVisible;
    [ObservableProperty]
    private bool _isBusy;

    public UsersManagementViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        ChargerUtilisateurs();
    }

    [RelayCommand]
    private void ChargerUtilisateurs()
    {
        try
        {
            List<Utilisateur> utilisateurs = _databaseService.LireUtilisateurs();
            Utilisateurs.Clear();
            foreach (var utilisateur in utilisateurs)
            {
                Utilisateurs.Add(utilisateur);
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Impossible de charger les utilisateurs.";
            IsErrorVisible = true;
        }
    }

    [RelayCommand]
    private void SupprimerUtilisateur(Utilisateur utilisateur)
    {
        try
        {
            bool supprimer = _databaseService.SupprimerUtilisateur(utilisateur.Id);
            if (supprimer)
            {
                Utilisateurs.Remove(utilisateur);
                SuccessMessage = $"Utilisateur {utilisateur.Prenom} {utilisateur.Nom} supprimé avec succès.";
                IsSuccessVisible = true;
                ErrorMessage = string.Empty;
                IsErrorVisible = false;
            }
            else
            {
                ErrorMessage = "Impossible de supprimer l'utilisateur.";
                IsErrorVisible = true;
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Une erreur est survenue pendant la suppression de l'utilisateur.";
            IsErrorVisible = true;
        }
    }

    [RelayCommand]
    private void AffichageFormulaireDeCreation()
    {
        ErrorMessage = string.Empty;
        IsErrorVisible = false;
        SuccessMessage = string.Empty;
        IsSuccessVisible = false;
        ReinitialiserFormulaire();
    }

    [RelayCommand]
    private Task AnnulerCreationAsync()
    {
        ErrorMessage = string.Empty;
        IsErrorVisible = false;
        SuccessMessage = string.Empty;
        IsSuccessVisible = false;
        ReinitialiserFormulaire();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task CreationUtilisateurAsync()
    {
        var nom = Nom;
        var prenom = Prenom;
        var email = Email;
        var login = Login;
        var motDePasse = MotDePasse;
        var adresse = Adresse;
        var codePostal = CodePostal;
        var ville = Ville;

        ErrorMessage = string.Empty;
        IsErrorVisible = false;
        SuccessMessage = string.Empty;
        IsSuccessVisible = false;

        if (string.IsNullOrWhiteSpace(nom) ||
            string.IsNullOrWhiteSpace(prenom) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(login) ||
            string.IsNullOrWhiteSpace(motDePasse) ||
            string.IsNullOrWhiteSpace(NumeroTelephone) ||
            string.IsNullOrWhiteSpace(adresse) ||
            string.IsNullOrWhiteSpace(codePostal) ||
            string.IsNullOrWhiteSpace(ville))
        {
            ErrorMessage = "Veuillez remplir tous les champs.";
            IsErrorVisible = true;
            return Task.CompletedTask;
        }

        if (!EmailValide(email))
        {
            ErrorMessage = "L'email n'est pas valide.";
            IsErrorVisible = true;
            return Task.CompletedTask;
        }

        if (!int.TryParse(NumeroTelephone, out int numeroTelephone))
        {
            ErrorMessage = "Le numero de téléphone doit être numérique.";
            IsErrorVisible = true;
            return Task.CompletedTask;
        }

        if (!int.TryParse(CodePostal, out int codePostalNumerique))
        {
            ErrorMessage = "Le code postal doit être numérique.";
            IsErrorVisible = true;
            return Task.CompletedTask;
        }

        try
        {
            IsBusy = true;

            var hashMotDePasse = _databaseService.HashMotDePasse(motDePasse);
             _databaseService.CreationUtilisateur(
                nom,
                prenom,
                email,
                login,
                hashMotDePasse,
                numeroTelephone,
                adresse,
                codePostalNumerique.ToString(),
                ville,
                Gestionnaire);

            SuccessMessage = $"Utilisateur {login} crée avec succès.";
            IsSuccessVisible = true;

            ReinitialiserFormulaire();
            ChargerUtilisateurs();
        }
        catch (Exception)
        {
            ErrorMessage = "Une erreur est survenue pendant la création de l'utilisateur.";
            IsErrorVisible = true;
        }
        finally
        {
            IsBusy = false;
        }

        return Task.CompletedTask;
    }

    private void ReinitialiserFormulaire()
    {
        Nom = string.Empty;
        Prenom = string.Empty;
        Email = string.Empty;
        Login = string.Empty;
        MotDePasse = string.Empty;
        NumeroTelephone = string.Empty;
        Adresse = string.Empty;
        CodePostal = string.Empty;
        Ville = string.Empty;
        Gestionnaire = false;
    }

    private static bool EmailValide(string email)
    {
        var emailTrim = email.Trim();
        return System.Text.RegularExpressions.Regex.IsMatch(emailTrim, @"^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])");
    }
}