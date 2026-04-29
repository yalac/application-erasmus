namespace ErAtlas.Model;

public class Utilisateur
{
    public int Id { get; set; }
    public string? Login { get; set; }
    public string? MotDePasse { get; set; } 
    public string? Nom { get; set; } 
    public string? Prenom { get; set; } 
    public string? Email { get; set; } 
    public int NumeroDeTelephone { get; set; } 
    public string? Adresse { get; set; } 
    public int CodePostal { get; set; } 
    public string? Ville { get; set; } 
    public bool Gestionnaire { get; set; } 
}
