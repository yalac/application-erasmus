using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErAtlas.Database;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;

namespace ErAtlas.ViewModels;

public partial class MyTravelViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<TravelCardItem> Travels { get; } = new();

    public MyTravelViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        SeedTravelCards();
    }

    [RelayCommand]
    private void ShowDetails(TravelCardItem? travel)
    {
        if (travel is null)
        {
            return;
        }

        SelectedTravelMessage = $"Details: {travel.RouteText}";
    }

    [RelayCommand]
    private void EditTravel(TravelCardItem? travel)
    {
        if (travel is null)
        {
            return;
        }

        SelectedTravelMessage = $"Edit: {travel.RouteText}";
    }

    [ObservableProperty]
    private string _selectedTravelMessage = string.Empty;

    private void SeedTravelCards()
    {
        Travels.Add(new TravelCardItem(
            iconGlyph: "train.svg",
            statusText: "Prevu",
            statusBackgroundColor: Color.FromArgb("#173A8A"),
            statusTextColor: Color.FromArgb("#7CC6FF"),
            routeText: "Paris -> Lyon",
            dateText: "10/03/2026",
            timeText: "14:30",
            segmentsText: string.Empty,
            viaText: string.Empty,
            transportModes: new[] { "Train" }
        ));

        Travels.Add(new TravelCardItem(
            iconGlyph: "plane.svg",
            statusText: "Prevu",
            statusBackgroundColor: Color.FromArgb("#173A8A"),
            statusTextColor: Color.FromArgb("#7CC6FF"),
            routeText: "Paris -> Tartu",
            dateText: "15/03/2026",
            timeText: "09:15",
            segmentsText: "2 segments",
            viaText: "via Tallinn",
            transportModes: new[] { "Avion", "Bus" }
        ));

        Travels.Add(new TravelCardItem(
            iconGlyph: "✈️",
            statusText: "Termine",
            statusBackgroundColor: Color.FromArgb("#105244"),
            statusTextColor: Color.FromArgb("#2AF5AF"),
            routeText: "Lyon -> Marseille",
            dateText: "05/03/2026",
            timeText: "09:15",
            segmentsText: string.Empty,
            viaText: string.Empty,
            transportModes: Array.Empty<string>()
        ));

        Travels.Add(new TravelCardItem(
            iconGlyph: "🚌",
            statusText: "En cours",
            statusBackgroundColor: Color.FromArgb("#3B246E"),
            statusTextColor: Color.FromArgb("#C08CFF"),
            routeText: "Bordeaux -> Toulouse",
            dateText: "25/03/2026",
            timeText: "11:10",
            segmentsText: string.Empty,
            viaText: string.Empty,
            transportModes: new[] { "Bus" }
        ));
    }
}

public class TravelCardItem
{
    public TravelCardItem(
        string iconGlyph,
        string statusText,
        Color statusBackgroundColor,
        Color statusTextColor,
        string routeText,
        string dateText,
        string timeText,
        string segmentsText,
        string viaText,
        IEnumerable<string> transportModes)
    {
        IconGlyph = iconGlyph;
        StatusText = statusText;
        StatusBackgroundColor = statusBackgroundColor;
        StatusTextColor = statusTextColor;
        RouteText = routeText;
        DateText = dateText;
        TimeText = timeText;
        SegmentsText = segmentsText;
        ViaText = viaText;
        TransportModes = new ObservableCollection<string>(transportModes);
    }

    public string IconGlyph { get; }
    public string StatusText { get; }
    public Color StatusBackgroundColor { get; }
    public Color StatusTextColor { get; }
    public string RouteText { get; }
    public string DateText { get; }
    public string TimeText { get; }
    public string SegmentsText { get; }
    public string ViaText { get; }
    public ObservableCollection<string> TransportModes { get; }

    public bool HasSegments => !string.IsNullOrWhiteSpace(SegmentsText);
    public bool HasVia => !string.IsNullOrWhiteSpace(ViaText);
    public bool HasTransportModes => TransportModes.Count > 0;
}

