using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ErAtlas.Database;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    
    [ObservableProperty]
    private ObservableCollection<Trajet> _trajets = new();
    
    [ObservableProperty]
    private ObservableCollection<Trajet> _trajetsRecent = new();
    
    public int TotalTrajets => Trajets.Count;
    public int TrajetsTerminesCount => Trajets.Count(t => t.Statut == "Terminé");
    public int TrajetsEnCoursCount => Trajets.Count(t => t.Statut == "A l'heure" || t.Statut == "En retard");


    public MainPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        ChargerTrajets();
        ChargerTrajetsRecents();
        
        LoginViewModel.OnUserLoggedOut += () =>
        {
            TrajetsRecent.Clear();
        };
        LoginViewModel.OnUserLoggedIn += (user) =>
        {
            ChargerTrajetsRecents();
        };
    }

    private void ChargerTrajets()
    {
        try
        {
            Trajets.Clear();
            foreach (var trajet in _databaseService.LireTrajets())
            {
                Trajets.Add(trajet);
            }
        }
        catch (Exception)
        {
            
        }
    }
    
    private void ChargerTrajetsRecents()
    {
        try
        {
            TrajetsRecent.Clear();

            if (LoginViewModel.IsLoggedIn && LoginViewModel.CurrentUser != null)
            {
                if (LoginViewModel.IsGestionnaire)
                {
                    // Pour un gestionnaire : 3 trajets avec la date de départ la plus proche de la date du jour
                    var allTrajets = _databaseService.LireTrajets()
                        .OrderBy(t => Math.Abs((t.DateDepart - DateTime.Today).TotalDays))
                        .Take(3)
                        .ToList();

                    // Pour chaque trajet, on charge les noms des villes et le type de transport
                    foreach (var trajet in allTrajets)
                    {
                        // Récupère les noms des villes
                        var lieuDepart = _databaseService.LireLieux().FirstOrDefault(l => l.IdLieu == trajet.IDLieuDepart);
                        var lieuArrivee = _databaseService.LireLieux().FirstOrDefault(l => l.IdLieu == trajet.IDLieuArrivee);
                        var transport = _databaseService.LireTransports().FirstOrDefault(t => t.IdTransport == trajet.IDTransport);

                        // Crée un nouvel objet Trajet avec les noms des villes
                        var trajetWithDetails = new Trajet
                        {
                            IDTrajet = trajet.IDTrajet,
                            DateDepart = trajet.DateDepart,
                            HeureDepart = trajet.HeureDepart,
                            DateArrivee = trajet.DateArrivee,
                            HeureArrivee = trajet.HeureArrivee,
                            Statut = trajet.Statut,
                            VilleDepart = lieuDepart?.Ville ?? "Inconnu",
                            VilleArrivee = lieuArrivee?.Ville ?? "Inconnu",
                            TypeTransport = transport?.TypeTransport ?? "Inconnu"
                        };

                        TrajetsRecent.Add(trajetWithDetails);
                    }
                }
                else
                {
                    // Pour un utilisateur normal : 2-3 derniers trajets qui le concernent
                    var userTrajets = _databaseService.GetVoyage(LoginViewModel.CurrentUser.Id)
                        .OrderByDescending(t => t.DateDepart)
                        .Take(3)
                        .ToList();

                    foreach (var trajet in userTrajets)
                    {
                        TrajetsRecent.Add(trajet);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Gérer l'erreur si nécessaire
        }
    }
    
    public void RefreshTrajets()
    {
        ChargerTrajets();
        OnPropertyChanged(nameof(TotalTrajets));
        OnPropertyChanged(nameof(TrajetsTerminesCount));
        OnPropertyChanged(nameof(TrajetsEnCoursCount));
    }
}