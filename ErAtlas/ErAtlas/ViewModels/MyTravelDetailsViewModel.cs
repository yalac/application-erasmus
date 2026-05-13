using CommunityToolkit.Mvvm.ComponentModel;
using ErAtlas.Database;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class MyTravelDetailsViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    
    [ObservableProperty] 
    private Trajet _trajet = new();

    public MyTravelDetailsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Travel", out var travelObj) && travelObj is Trajet travel)
        {
            // Charger les détails complets du trajet depuis la base de données
            LoadTravelDetails(travel);
        }
    }

    private void LoadTravelDetails(Trajet travel)
    {
        try
        {
            // Charger les détails complets du trajet
            var allTravels = _databaseService.LireTrajets();
            var fullTravel = allTravels.FirstOrDefault(t => t.IDTrajet == travel.IDTrajet) ?? travel;

            // Charger les lieux
            var lieux = _databaseService.LireLieux();
            if (fullTravel.IDLieuDepart > 0)
            {
                fullTravel.LieuDepart = lieux.FirstOrDefault(l => l.IdLieu == fullTravel.IDLieuDepart);
            }

            if (fullTravel.IDLieuArrivee > 0)
            {
                fullTravel.LieuArrivee = lieux.FirstOrDefault(l => l.IdLieu == fullTravel.IDLieuArrivee);
            }

            // Charger le transport
            var transports = _databaseService.LireTransports();
            if (fullTravel.IDTransport > 0)
            {
                fullTravel.Transport = transports.FirstOrDefault(t => t.IdTransport == fullTravel.IDTransport);
                if (fullTravel.Transport != null)
                {
                    fullTravel.TypeTransport = fullTravel.Transport.TypeTransport;
                }
            }

            Trajet = fullTravel;
        }
        catch
        {
            // En cas d'erreur, utiliser le trajet fourni
            Trajet = travel;
        }
    }
}
