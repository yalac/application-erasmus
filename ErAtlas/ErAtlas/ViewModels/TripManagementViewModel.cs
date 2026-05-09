using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class TripManagementViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private int _lieuIdEnModification;
    private int _transportIdEnModification;
    private int _trajetIdEnModification;

    [ObservableProperty]
    private ObservableCollection<Lieu> _lieux = new();

    [ObservableProperty]
    private ObservableCollection<Transport> _transports = new();
    
    [ObservableProperty]
    private ObservableCollection<Trajet> _trajets = new();

    [ObservableProperty]
    private string _nomLieu = string.Empty;

    [ObservableProperty]
    private string _adresseLieu = string.Empty;

    [ObservableProperty]
    private string _codePostalLieu = string.Empty;

    [ObservableProperty]
    private string _villeLieu = string.Empty;

    [ObservableProperty]
    private string _paysLieu = string.Empty;

    [ObservableProperty]
    private string _typeTransport = string.Empty;

    [ObservableProperty]
    private string _capaciteTransport = string.Empty;

    [ObservableProperty]
    private string _immatriculationTransport = string.Empty;

    [ObservableProperty]
    private string _descriptionTransport = string.Empty;
    
    [ObservableProperty]
    private DateTime? _dateDepart = null;
    
    [ObservableProperty]
    private TimeSpan? _heureDepart = null;
    
    [ObservableProperty]
    private DateTime? _dateArrivee = null;
    
    [ObservableProperty]
    private TimeSpan? _heureArrivee = null;
    
    [ObservableProperty]
    private string _statut = string.Empty;

    [ObservableProperty] 
    private int _idLieuArrivee = 0;

    [ObservableProperty] 
    private int _idLieuDepart = 0;
    
    [ObservableProperty] 
    private int _idTransport = 0;

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

    [ObservableProperty]
    private bool _isLieuModificationMode;

    [ObservableProperty]
    private bool _isTransportModificationMode;
    
    [ObservableProperty]
    private bool _isTrajetsModificationMode;

    public string TitreFormulaireLieu => IsLieuModificationMode ? "Modifier le lieu" : "Créer un lieu";

    public string TexteBoutonLieu => IsLieuModificationMode ? "Enregistrer" : "Créer";

    public string TitreFormulaireTransport => IsTransportModificationMode ? "Modifier le transport" : "Créer un transport";

    public string TexteBoutonTransport => IsTransportModificationMode ? "Enregistrer" : "Créer";
    
    public string TitreFormulaireTrajet => IsTrajetsModificationMode ? "Modifier le trajet" : "Créer un trajet";

    public string TexteBoutonTrajet => IsTrajetsModificationMode ? "Enregistrer" : "Créer";

    public TripManagementViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        ChargerLieux();
        ChargerTransports();
        ChargerTrajets();
    }

    partial void OnIsLieuModificationModeChanged(bool value)
    {
        OnPropertyChanged(nameof(TitreFormulaireLieu));
        OnPropertyChanged(nameof(TexteBoutonLieu));
    }

    partial void OnIsTransportModificationModeChanged(bool value)
    {
        OnPropertyChanged(nameof(TitreFormulaireTransport));
        OnPropertyChanged(nameof(TexteBoutonTransport));
    }
    
    partial void OnIsTrajetsModificationModeChanged(bool value)
    {
        OnPropertyChanged(nameof(TitreFormulaireTrajet));
        OnPropertyChanged(nameof(TexteBoutonTrajet));
    }

    private void ChargerLieux()
    {
        try
        {
            Lieux.Clear();
            foreach (var lieu in _databaseService.LireLieux())
            {
                Lieux.Add(lieu);
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Impossible de charger les lieux.";
            IsErrorVisible = true;
        }
    }

    private void ChargerTransports()
    {
        try
        {
            Transports.Clear();
            foreach (var transport in _databaseService.LireTransports())
            {
                Transports.Add(transport);
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Impossible de charger les transports.";
            IsErrorVisible = true;
        }
    }
    
    private void ChargerTrajets()
    {
        try
        {
            Trajets.Clear();
            foreach (var trajets in _databaseService.LireTrajets())
            {
                Trajets.Add(trajets);
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Impossible de charger les trajets.";
            IsErrorVisible = true;
        }
    }

    [RelayCommand]
    private void AffichageFormulaireLieuCreation()
    {
        ResetMessages();
        IsLieuModificationMode = false;
        _lieuIdEnModification = 0;
        ReinitialiserFormulaireLieu();
    }

    [RelayCommand]
    private void ModifierLieu(Lieu? lieu)
    {
        ResetMessages();

        if (lieu is null)
        {
            ErrorMessage = "Aucun lieu sélectionné pour la modification.";
            IsErrorVisible = true;
            return;
        }

        IsLieuModificationMode = true;
        _lieuIdEnModification = lieu.IdLieu;
        NomLieu = lieu.Nom;
        AdresseLieu = lieu.Adresse;
        CodePostalLieu = lieu.CodePostal.ToString();
        VilleLieu = lieu.Ville;
        PaysLieu = lieu.Pays;
    }

    [RelayCommand]
    private async Task EnregistrerLieuAsync()
    {
        ResetMessages();

        if (string.IsNullOrWhiteSpace(NomLieu) ||
            string.IsNullOrWhiteSpace(AdresseLieu) ||
            string.IsNullOrWhiteSpace(CodePostalLieu) ||
            string.IsNullOrWhiteSpace(VilleLieu) ||
            string.IsNullOrWhiteSpace(PaysLieu))
        {
            ErrorMessage = "Veuillez remplir tous les champs du lieu.";
            IsErrorVisible = true;
            return;
        }

        if (!int.TryParse(CodePostalLieu, out int codePostalNumerique))
        {
            ErrorMessage = "Le code postal du lieu doit être numérique.";
            IsErrorVisible = true;
            return;
        }

        try
        {
            IsBusy = true;

            var lieuEnregistre = IsLieuModificationMode
                ? _databaseService.ModifierLieu(_lieuIdEnModification, NomLieu, AdresseLieu, codePostalNumerique.ToString(), VilleLieu, PaysLieu)
                : _databaseService.CreationLieu(NomLieu, AdresseLieu, codePostalNumerique.ToString(), VilleLieu, PaysLieu);

            SuccessMessage = IsLieuModificationMode
                ? $"Lieu {lieuEnregistre.Nom} modifié avec succès."
                : $"Lieu {lieuEnregistre.Nom} créé avec succès.";
            IsSuccessVisible = true;

            ReinitialiserFormulaireLieu();
            ChargerLieux();
        }
        catch (Exception)
        {
            ErrorMessage = "Une erreur est survenue pendant l'enregistrement du lieu.";
            IsErrorVisible = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SupprimerLieuAsync(Lieu? lieu)
    {
        ResetMessages();

        if (lieu is null)
        {
            return;
        }

        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is null)
        {
            ErrorMessage = "Impossible d'afficher la confirmation.";
            IsErrorVisible = true;
            return;
        }

        bool confirmation = await page.DisplayAlert("Confirmer la suppression", $"Voulez-vous supprimer le lieu {lieu.Nom} ?", "Oui", "Non");
        if (!confirmation)
        {
            return;
        }

        bool supprime = _databaseService.SupprimerLieu(lieu.IdLieu);
        if (supprime)
        {
            Lieux.Remove(lieu);
            SuccessMessage = $"Lieu {lieu.Nom} supprimé avec succès.";
            IsSuccessVisible = true;
        }
        else
        {
            ErrorMessage = "Impossible de supprimer le lieu.";
            IsErrorVisible = true;
        }
    }

    /* -------------------------------------------------------- */
    [RelayCommand]
    private void AffichageFormulaireTrajetCreation()
    {
        ResetMessages();
        IsTrajetsModificationMode = false;
        _trajetIdEnModification = 0;
        ReinitialiserFormulaireTrajet();
    }

    [RelayCommand]
    private void ModifierTrajet(Trajet? trajet)
    {
        ResetMessages();

        if (trajet is null)
        {
            ErrorMessage = "Aucun trajet sélectionné pour la modification.";
            IsErrorVisible = true;
            return;
        }

        IsTrajetsModificationMode = true;
        _trajetIdEnModification = trajet.IDTrajet;
        DateDepart = trajet.DateDepart;
        HeureDepart = trajet.HeureDepart;
        DateArrivee = trajet.DateArrivee;
        HeureArrivee = trajet.HeureArrivee;
        Statut = trajet.Statut;
        IdLieuDepart = trajet.IDLieuDepart;
        IdLieuArrivee = trajet.IDLieuArrivee;
        IdTransport = trajet.IDTransport;
    }

    [RelayCommand]
    private async Task EnregistrerTrajetAsync()
    {
        ResetMessages();

        // Vérification des champs obligatoires pour un trajet
        if (DateDepart == null || HeureDepart == null ||
            DateArrivee == null || HeureArrivee == null ||
            string.IsNullOrWhiteSpace(Statut) ||
            IdLieuDepart == 0 || IdLieuArrivee == 0 || IdTransport == 0)
        {
            ErrorMessage = "Veuillez remplir tous les champs du trajet (les IDs de lieu et transport ne peuvent pas être à 0).";
            IsErrorVisible = true;
            return;
        }

        try
        {
            IsBusy = true;

            var trajetEnregistre = IsTrajetsModificationMode
                ? _databaseService.ModifierTrajet(
                    _trajetIdEnModification,
                    DateDepart.Value,
                    HeureDepart.Value,
                    DateArrivee.Value,
                    HeureArrivee.Value,
                    Statut,
                    IdLieuArrivee,
                    IdLieuDepart,
                    IdTransport)
                : _databaseService.CreationTrajet(
                    DateDepart.Value,
                    HeureDepart.Value,
                    DateArrivee.Value,
                    HeureArrivee.Value,
                    Statut,
                    IdLieuArrivee,
                    IdLieuDepart,
                    IdTransport);

            SuccessMessage = IsTrajetsModificationMode
                ? $"Trajet vers {IdLieuArrivee} modifié avec succès."
                : $"Trajet vers {IdLieuArrivee} créé avec succès.";
            IsSuccessVisible = true;

            ReinitialiserFormulaireTrajet();
            ChargerTrajets();
        }
        catch (Exception)
        {
            ErrorMessage = "Une erreur est survenue pendant l'enregistrement du trajet.";
            IsErrorVisible = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SupprimerTrajetAsync(Trajet? trajet)
    {
        ResetMessages();

        if (trajet is null)
        {
            return;
        }

        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is null)
        {
            ErrorMessage = "Impossible d'afficher la confirmation.";
            IsErrorVisible = true;
            return;
        }

        bool confirmation = await page.DisplayAlert(
            "Confirmer la suppression",
            $"Voulez-vous supprimer le trajet du {trajet.DateDepart:d} à {trajet.HeureDepart:t} ?",
            "Oui", "Non");

        if (!confirmation)
        {
            return;
        }

        bool supprime = _databaseService.SupprimerTrajet(trajet.IDTrajet);
        if (supprime)
        {
            Trajets.Remove(trajet);
            SuccessMessage = $"Trajet du {trajet.DateDepart:d} supprimé avec succès.";
            IsSuccessVisible = true;
        }
        else
        {
            ErrorMessage = "Impossible de supprimer le trajet.";
            IsErrorVisible = true;
        }
    }
    
    
    [RelayCommand]
    private void AffichageFormulaireTransportCreation()
    {
        ResetMessages();
        IsTransportModificationMode = false;
        _transportIdEnModification = 0;
        ReinitialiserFormulaireTransport();
    }

    [RelayCommand]
    private void ModifierTransport(Transport? transport)
    {
        ResetMessages();

        if (transport is null)
        {
            ErrorMessage = "Aucun transport sélectionné pour la modification.";
            IsErrorVisible = true;
            return;
        }

        IsTransportModificationMode = true;
        _transportIdEnModification = transport.IdTransport;
        TypeTransport = transport.TypeTransport;
        CapaciteTransport = transport.Capacite.ToString();
        ImmatriculationTransport = transport.Immatriculation;
        DescriptionTransport = transport.Description;
    }

    [RelayCommand]
    private async Task EnregistrerTransportAsync()
    {
        ResetMessages();

        if (string.IsNullOrWhiteSpace(TypeTransport) ||
            string.IsNullOrWhiteSpace(CapaciteTransport) ||
            string.IsNullOrWhiteSpace(ImmatriculationTransport) ||
            string.IsNullOrWhiteSpace(DescriptionTransport))
        {
            ErrorMessage = "Veuillez remplir tous les champs du transport.";
            IsErrorVisible = true;
            return;
        }

        if (!int.TryParse(CapaciteTransport, out int capaciteNumerique))
        {
            ErrorMessage = "La capacité du transport doit être numérique.";
            IsErrorVisible = true;
            return;
        }

        try
        {
            IsBusy = true;

            var transportEnregistre = IsTransportModificationMode
                ? _databaseService.ModifierTransport(_transportIdEnModification, TypeTransport, capaciteNumerique, ImmatriculationTransport, DescriptionTransport)
                : _databaseService.CreationTransport(TypeTransport, capaciteNumerique, ImmatriculationTransport, DescriptionTransport);

            SuccessMessage = IsTransportModificationMode
                ? $"Transport {transportEnregistre.TypeTransport} modifié avec succès."
                : $"Transport {transportEnregistre.TypeTransport} créé avec succès.";
            IsSuccessVisible = true;

            ReinitialiserFormulaireTransport();
            ChargerTransports();
        }
        catch (Exception)
        {
            ErrorMessage = "Une erreur est survenue pendant l'enregistrement du transport.";
            IsErrorVisible = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SupprimerTransportAsync(Transport? transport)
    {
        ResetMessages();

        if (transport is null)
        {
            return;
        }

        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is null)
        {
            ErrorMessage = "Impossible d'afficher la confirmation.";
            IsErrorVisible = true;
            return;
        }

        bool confirmation = await page.DisplayAlert("Confirmer la suppression", $"Voulez-vous supprimer le transport {transport.TypeTransport} ?", "Oui", "Non");
        if (!confirmation)
        {
            return;
        }

        bool supprime = _databaseService.SupprimerTransport(transport.IdTransport);
        if (supprime)
        {
            Transports.Remove(transport);
            SuccessMessage = $"Transport {transport.TypeTransport} supprimé avec succès.";
            IsSuccessVisible = true;
        }
        else
        {
            ErrorMessage = "Impossible de supprimer le transport.";
            IsErrorVisible = true;
        }
    }

    private void ReinitialiserFormulaireLieu()
    {
        NomLieu = string.Empty;
        AdresseLieu = string.Empty;
        CodePostalLieu = string.Empty;
        VilleLieu = string.Empty;
        PaysLieu = string.Empty;
    }

    private void ReinitialiserFormulaireTransport()
    {
        TypeTransport = string.Empty;
        CapaciteTransport = string.Empty;
        ImmatriculationTransport = string.Empty;
        DescriptionTransport = string.Empty;
    }
    
    private void ReinitialiserFormulaireTrajet()
    {
        DateDepart = null;
        HeureDepart = null;
        DateArrivee = null;
        HeureArrivee = null;
        Statut = string.Empty;
        IdLieuArrivee = 0;
        IdLieuDepart = 0;
        IdTransport = 0;
    }

    private void ResetMessages()
    {
        ErrorMessage = string.Empty;
        IsErrorVisible = false;
        SuccessMessage = string.Empty;
        IsSuccessVisible = false;
    }
}