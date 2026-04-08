using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;

namespace ErAtlas.ViewModels;

public partial class UsersManagementViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

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
    private bool _formulaireVisible;
    [ObservableProperty]
    private bool _isBusy;

    public UsersManagementViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _successMessage = string.Empty;
    }

    [RelayCommand]
    private void AffichageFormulaireDeCreation()
    {
        FormulaireVisible = !FormulaireVisible;
    }

    [RelayCommand]
    private void AnnulerCreation()
    {
        ReinitialiserFormulaire();
        FormulaireVisible = false;
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
    
    [RelayCommand]
    private void CreationUtilisateur()
    {
        var nom = Nom;
        var prenom = Prenom;
        var email = Email;
        var login = Login;
        var motDePasse = MotDePasse;
        var adresse = Adresse;
        var codePostal = CodePostal;
        var ville = Ville;

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
            return;
        }
        
        if (!EmailValide(email))
        {
            ErrorMessage = "L'email n'est pas valide.";
            IsErrorVisible = true;
            return;
        }
        
        if (!int.TryParse(NumeroTelephone, out int numeroTelephone))
        {
            ErrorMessage = "Le numero de téléphone doit être numérique.";
            IsErrorVisible = true;
            return;
        }

        if (!int.TryParse(CodePostal, out int codePostalNumerique))
        {
            ErrorMessage = "Le code postal doit être numérique.";
            IsErrorVisible = true;
            return;
        }

        var hashMotDePasse = _databaseService.HashMotDePasse(motDePasse);

        try
        {
            IsBusy = true;

            var utilisateurCree = _databaseService.CreationUtilisateur(
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

            SuccessMessage = $"Utilisateur {utilisateurCree.Login} crée avec succès.";
            IsSuccessVisible = true;
            
            ReinitialiserFormulaire();
            FormulaireVisible = false;
        }
        catch (Exception)
        {
            ErrorMessage = "Une erreur est survenue pendant la création de l'utilisateur.";
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private static bool EmailValide(string email)
    {
        var emailTrim = email.Trim();
        return Regex.IsMatch(emailTrim, @"^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])");
    }
}