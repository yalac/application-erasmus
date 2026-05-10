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
    
    public int TotalTrajets => Trajets.Count;
    public int TrajetsTerminesCount => Trajets.Count(t => t.Statut == "Terminé");
    public int TrajetsEnCoursCount => Trajets.Count(t => t.Statut == "A l'heure" || t.Statut == "En retard");


    public MainPageViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        ChargerTrajets();
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
    
    public void RefreshTrajets()
    {
        ChargerTrajets();
        OnPropertyChanged(nameof(TotalTrajets));
        OnPropertyChanged(nameof(TrajetsTerminesCount));
        OnPropertyChanged(nameof(TrajetsEnCoursCount));
    }
}