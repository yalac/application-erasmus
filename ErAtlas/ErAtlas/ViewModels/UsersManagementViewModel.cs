using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class UsersManagementViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private int _utilisateurIdEnModification;
    private bool _isModificationMode;
    private string _motDePasseOriginal = string.Empty;
    
    public bool HasAccess => LoginViewModel.IsLoggedIn && LoginViewModel.IsGestionnaire;
    public bool IsAccessDenied => !HasAccess;

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

    public bool IsModificationMode
    {
        get => _isModificationMode;
        set => _isModificationMode = value;
    }


    public UsersManagementViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        ChargerUtilisateurs();
        
        // Écouter les changements de connexion/déconnexion pour mettre à jour l'accès
        LoginViewModel.OnUserLoggedIn += OnUserLoggedIn;
        LoginViewModel.OnUserLoggedOut += OnUserLoggedOut;
    }

    private void OnUserLoggedIn(Utilisateur? _)
    {
        OnPropertyChanged(nameof(HasAccess));
        OnPropertyChanged(nameof(IsAccessDenied));
        ChargerUtilisateurs();
    }

    private void OnUserLoggedOut()
    {
        OnPropertyChanged(nameof(HasAccess));
        OnPropertyChanged(nameof(IsAccessDenied));
        Utilisateurs.Clear();
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
    private async Task SupprimerUtilisateurAsync(Utilisateur? utilisateur)
    {
        bool confirmation = false;

        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is null)
        {
            ErrorMessage = "Impossible d'afficher la confirmation.";
            IsErrorVisible = true;
            return;
        }

        confirmation = await page.DisplayAlert("Confirmer la suppression", $"Voulez-vous supprimer {utilisateur.Prenom} {utilisateur.Nom} ?", "Oui", "Non");

        if (!confirmation)
        {
            return;
        }

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

    [RelayCommand]
    private void AffichageFormulaireDeCreation()
    {
        ErrorMessage = string.Empty;
        IsErrorVisible = false;
        SuccessMessage = string.Empty;
        IsSuccessVisible = false;
        IsModificationMode = false;
        _utilisateurIdEnModification = 0;
        ReinitialiserFormulaire();
    }

    [RelayCommand]
    private Task AnnulerCreationAsync()
    {
        ErrorMessage = string.Empty;
        IsErrorVisible = false;
        SuccessMessage = string.Empty;
        IsSuccessVisible = false;
        IsModificationMode = false;
        _utilisateurIdEnModification = 0;
        ReinitialiserFormulaire();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void ModificationUtilisateur(Utilisateur? utilisateur)
    {
        ErrorMessage = string.Empty;
        IsErrorVisible = false;
        SuccessMessage = string.Empty;
        IsSuccessVisible = false;

        if (utilisateur is null)
        {
            ErrorMessage = "Aucun utilisateur sélectionné pour la modification.";
            IsErrorVisible = true;
            return;
        }

        IsModificationMode = true;
        _utilisateurIdEnModification = utilisateur.Id;
        _motDePasseOriginal = string.Empty; // On n'a pas accès au mot de passe original pour des raisons de sécurité

        Nom = utilisateur.Nom;
        Prenom = utilisateur.Prenom;
        Email = utilisateur.Email;
        Login = utilisateur.Login;
        MotDePasse = string.Empty;
        NumeroTelephone = utilisateur.NumeroDeTelephone.ToString();
        Adresse = utilisateur.Adresse;
        CodePostal = utilisateur.CodePostal.ToString();
        Ville = utilisateur.Ville;
        Gestionnaire = utilisateur.Gestionnaire;
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

        // En création, tous les champs sont obligatoires
        // En modification, le mot de passe est optionnel
        if (string.IsNullOrWhiteSpace(nom) ||
            string.IsNullOrWhiteSpace(prenom) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(login) ||
            (!IsModificationMode && string.IsNullOrWhiteSpace(motDePasse)) ||
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

            Utilisateur utilisateurEnregistre;

            if (IsModificationMode)
            {
                // En modification : si le mot de passe est vide, ne pas le modifier
                if (string.IsNullOrWhiteSpace(motDePasse))
                {
                    utilisateurEnregistre = _databaseService.ModifierUtilisateur(
                        _utilisateurIdEnModification,
                        nom,
                        prenom,
                        email,
                        login,
                        numeroTelephone,
                        adresse,
                        codePostalNumerique.ToString(),
                        ville,
                        Gestionnaire);
                }
                else
                {
                    var hashMotDePasse = _databaseService.HashMotDePasse(motDePasse);
                    utilisateurEnregistre = _databaseService.ModifierUtilisateur(
                        _utilisateurIdEnModification,
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
                }
            }
            else
            {
                // En création : hasher le mot de passe obligatoire
                var hashMotDePasse = _databaseService.HashMotDePasse(motDePasse);
                utilisateurEnregistre = _databaseService.CreationUtilisateur(
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
            }

            SuccessMessage = IsModificationMode
                ? $"Utilisateur {utilisateurEnregistre.Login} modifié avec succès."
                : $"Utilisateur {utilisateurEnregistre.Login} crée avec succès.";
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
        IsModificationMode = false;
        _utilisateurIdEnModification = 0;
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