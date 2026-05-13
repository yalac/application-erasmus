using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;
using ErAtlas.Model;
using System.Collections.ObjectModel;

namespace ErAtlas.ViewModels;

public partial class MyTravelViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private List<Lieu> _lieux = new();
    private List<Transport> _transports = new();

    public ObservableCollection<Trajet> Travels { get; } = new();

    [ObservableProperty]
    private Trajet? _selectedTravel;

    [ObservableProperty]
    private string _selectedTravelMessage = string.Empty;

    public MyTravelViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        LoadReferences(); // Charger les lieux et transports une fois
        LoadTravels(); // Charge les trajets à l'initialisation

        // Écouter les changements de connexion/déconnexion
        LoginViewModel.OnUserLoggedIn += (user) =>
        {
            LoadReferences(); // Recharger les références au cas où
            LoadTravels();
        };
        
        LoginViewModel.OnUserLoggedOut += () =>
        {
            Travels.Clear();
            SelectedTravelMessage = string.Empty;
        };
    }

    // Charge les lieux et transports une fois pour réutilisation
    private void LoadReferences()
    {
        try
        {
            _lieux = _databaseService.LireLieux();
            _transports = _databaseService.LireTransports();
        }
        catch
        {
            _lieux = new();
            _transports = new();
        }
    }

    // Charge les trajets en fonction du rôle de l'utilisateur
    private void LoadTravels()
    {
        try
        {
            Travels.Clear();

            if (LoginViewModel.IsLoggedIn && LoginViewModel.CurrentUser != null)
            {
                if (LoginViewModel.IsGestionnaire)
                {
                    // Pour un gestionnaire : charge TOUS les trajets
                    var allTravels = _databaseService.LireTrajets();
                    foreach (var travel in allTravels)
                    {
                        Travels.Add(GetTravelWithDetails(travel));
                    }
                }
                else
                {
                    // Pour un voyageur : charge UNIQUEMENT ses trajets
                    var userTravels = _databaseService.GetVoyage(LoginViewModel.CurrentUser.Id);
                    foreach (var travel in userTravels)
                    {
                        // GetVoyage retourne déjà les villes, mais on ajoute les détails manquants
                        Travels.Add(GetTravelWithDetails(travel));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SelectedTravelMessage = $"Erreur lors du chargement des trajets : {ex.Message}";
        }
    }

    // Ajoute les détails manquants à un trajet (villes, icône, route)
    private Trajet GetTravelWithDetails(Trajet travel)
    {
        // Si les villes sont déjà chargées (via GetVoyage), on les conserve
        if (string.IsNullOrEmpty(travel.VilleDepart) || string.IsNullOrEmpty(travel.VilleArrivee))
        {
            if (travel.IDLieuDepart > 0)
            {
                var lieuDepart = _lieux.FirstOrDefault(l => l.IdLieu == travel.IDLieuDepart);
                travel.VilleDepart = lieuDepart?.Ville ?? "Inconnu";
            }
            
            if (travel.IDLieuArrivee > 0)
            {
                var lieuArrivee = _lieux.FirstOrDefault(l => l.IdLieu == travel.IDLieuArrivee);
                travel.VilleArrivee = lieuArrivee?.Ville ?? "Inconnu";
            }
        }

        // Ajoute le type de transport si manquant
        if (string.IsNullOrEmpty(travel.TypeTransport) && travel.IDTransport > 0)
        {
            var transport = _transports.FirstOrDefault(t => t.IdTransport == travel.IDTransport);
            travel.TypeTransport = transport?.TypeTransport ?? "Inconnu";
        }

        // Ajoute la route (ex: "Paris → Lyon")
        travel.Route = $"{travel.VilleDepart ?? "Départ"} → {travel.VilleArrivee ?? "Arrivée"}";

        // Ajoute l'icône en fonction du type de transport
        travel.IconSource = GetIconSource(travel.TypeTransport);

        return travel;
    }

    // Détermine l'icône en fonction du type de transport
    private string GetIconSource(string? typeTransport)
    {
        return typeTransport?.ToLower() switch
        {
            "avion" => "avion.svg",
            "train" => "train.svg",
            "bus" => "bus.svg",
            "bateau" => "bateau.svg",
            _ => "train.svg" // Icône par défaut
        };
    }

    // Rafraîchit la liste des trajets
    public void RefreshTravels()
    {
        LoadTravels();
    }

    // Commande pour afficher les détails d'un trajet
    [RelayCommand]
    private void ShowDetails(Trajet travel)
    {
        SelectedTravel = travel;
        SelectedTravelMessage = $"Détails du trajet : {travel.Route}";
        // Ici, tu peux naviguer vers une page de détails si nécessaire
    }

    // Commande pour modifier un trajet
    [RelayCommand]
    private void EditTravel(Trajet travel)
    {
        SelectedTravel = travel;
        SelectedTravelMessage = $"Modification du trajet : {travel.Route}";

        // Après modification, rafraîchit la liste
        RefreshTravels();
    }
}



