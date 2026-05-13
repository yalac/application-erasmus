namespace ErAtlas.Model;

public class Trajet
{
    public int IDTrajet { get; set; }
    public DateTime DateDepart { get; set; }
    public TimeSpan HeureDepart { get; set; }
    public DateTime DateArrivee { get; set; }
    public TimeSpan HeureArrivee { get; set; }
    public string Statut { get; set; } = string.Empty;
    public int IDLieuArrivee { get; set; }
    public int IDLieuDepart { get; set; }
    public int IDTransport { get; set; }
    public Lieu? LieuDepart { get; set; }
    public Lieu? LieuArrivee { get; set; }
    public Transport? Transport { get; set; }
    public string VilleDepart { get; set; } = string.Empty;
    public string VilleArrivee { get; set; } = string.Empty;
    public string TypeTransport { get; set; } = string.Empty;

    private string _iconSource = string.Empty;
    private string _route = string.Empty;

    public string IconSource
    {
        get => string.IsNullOrWhiteSpace(_iconSource) ? GetIconSourceFromTransport(TypeTransport) : _iconSource;
        set => _iconSource = value ?? string.Empty;
    }

    public string Route
    {
        get => string.IsNullOrWhiteSpace(_route) ? $"{VilleDepart} -> {VilleArrivee}" : _route;
        set => _route = value ?? string.Empty;
    }

    // Méthode utilitaire pour joindre les types de transport à des icônes
    // Si c'est un bus → bus.png, si c'est un avion -> plane.png, etc.
    private string GetIconSourceFromTransport(string transport)
    {
        return transport.ToLowerInvariant() switch
        {
            "bus" => "bus.svg",
            "train" => "train.svg",
            "avion" => "avion.svg",
            "bateau" => "bateau.svg",
            _ => "train.svg"
        };
    }
}