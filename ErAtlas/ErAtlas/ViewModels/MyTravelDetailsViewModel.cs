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
            Trajet = travel;
        }
    }
}
