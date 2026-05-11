using System.Runtime.InteropServices.JavaScript;

namespace ErAtlas.Model;

public class Trajet
{
    public int IDTrajet { get; set; }
    public DateTime DateDepart { get; set; }
    public TimeSpan HeureDepart  { get; set; }
    public DateTime DateArrivee { get; set; }
    public TimeSpan HeureArrivee { get; set; }
    public string Statut { get; set; }
    public string VilleDepart{ get; set; }
    public string VilleArrivee { get; set; }                                
    public string TypeTransport { get; set; }

    public string IconSource => GetIconSourceFromTransport(TypeTransport);
    
    public string Route => $"{VilleDepart} → {VilleArrivee}";
    
    // Méthode utilitaire pour joindre les types de transport à des icônes
    // Si c'est un bus → bus.png, si c'est un avion -> plane.png, etc.
    private string GetIconSourceFromTransport(string transport)
    {
        return transport switch
        {
            "Bus" => "bus.png",
            "Train" => "train.png",
            "Avion" => "avion.png",
            "Bateau" => "bateau.png",
            _ => "dotnet_bot.png"    // fallback
        };
    }
}