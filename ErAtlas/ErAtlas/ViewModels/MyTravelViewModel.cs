using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using ErAtlas.Model;

namespace ErAtlas.ViewModels;

public partial class MyTravelViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    
    public ObservableCollection<Trajet> Travels { get; set; }

    public MyTravelViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Travels = new ObservableCollection<Trajet>(_databaseService.GetVoyage(1));
    }

    [RelayCommand]
    private void ShowDetails(Trajet travel)
    { }

    [RelayCommand]
    private void EditTravel(Trajet travel)
    { }

    [ObservableProperty]
    private string _selectedTravelMessage = string.Empty;
}


