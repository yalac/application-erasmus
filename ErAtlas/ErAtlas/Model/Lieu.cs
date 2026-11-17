namespace ErAtlas.Model;

public class Lieu
{
    public int IdLieu { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public int CodePostal { get; set; }
    public string Ville { get; set; } = string.Empty;
    public string Pays { get; set; } = string.Empty;
}

