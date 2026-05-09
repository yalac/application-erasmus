namespace ErAtlas.Model;

public class Transport
{
    public int IdTransport { get; set; }
    public string TypeTransport { get; set; } = string.Empty;
    public int Capacite { get; set; }
    public string Immatriculation { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}