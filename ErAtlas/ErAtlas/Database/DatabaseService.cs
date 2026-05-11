using System.Security.Cryptography;
using System.Text;
using ABI.Windows.Globalization.DateTimeFormatting;
using ErAtlas.Model;
using Microsoft.Data.SqlClient;

namespace ErAtlas.Database;

public class DatabaseService
{
    // Connexion à la base de données
    private SqlConnection _connection;
    private const string ConnectionString = "Server=localhost;Database=ErAtlas;User ID=sa;Password=Info76240#;TrustServerCertificate=True;";

    // Création du constructeur
    public DatabaseService()
    {
        _connection = new SqlConnection(ConnectionString);
        _connection.Open();
    }

    // Permet de lire la PS de vérification de la connexion et de renvoyé l'id, le mdp et le login si elle est vrai
    public Utilisateur? VerifUser(string login)
    {
        string query = "PS_VerificationConnexion";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@username", login);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return null;
        return new Utilisateur
        {
            Id = (int)reader["IDUtilisateur"],
            Login = (string)reader["Login"],
            MotDePasse = (string)reader["MotDePasse"]
        };
    }

    // Méthode pour hasher le mot de passe en SHA256
    public string HashMotDePasse(string motDePasse)
    {
        byte[] dataToHash = Encoding.UTF8.GetBytes(motDePasse);
        using SHA256 sha256Hash = SHA256.Create();
        byte[] hashBytes = sha256Hash.ComputeHash(dataToHash);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    //Vérification du mdp de l'utilisateur entre les hashs du mdp saisi et celui de la base de données
    public bool CheckUser(string login, string motDePasse)
    {
        try
        {
            Utilisateur? user = VerifUser(login);
            if (user is null)
                return false;

            string hashString = HashMotDePasse(motDePasse);
            string dbHash = user.MotDePasse?.Trim().Replace("-", "") ?? string.Empty;
            return string.Equals(dbHash, hashString, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    // Permet de lire la PS de création d'un utilisateur et de renvoyer ses informations si vrai
    public Utilisateur CreationUtilisateur(string nom, string prenom, string email, string login, string motDePasse, int numeroDeTelephone, string adresse, string codePostal, string ville, bool gestionnaire)
    {
        string query = "PS_CreationUtilisateur";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Nom", nom);
        command.Parameters.AddWithValue("@Prenom", prenom);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@Login", login);
        command.Parameters.AddWithValue("@MotDePasse", motDePasse);
        command.Parameters.AddWithValue("@NumeroTelephone", numeroDeTelephone);
        command.Parameters.AddWithValue("@Adresse", adresse);
        command.Parameters.AddWithValue("@CodePostal", codePostal);
        command.Parameters.AddWithValue("@Ville", ville);
        command.Parameters.AddWithValue("@Gestionnaire", gestionnaire);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Utilisateur
            {
                Id = (int)reader["IDUtilisateur"],
                Nom = (string)reader["Nom"],
                Prenom = (string)reader["Prenom"],
                Email = (string)reader["Email"],
                Login = (string)reader["Login"],
                MotDePasse = (string)reader["MotDePasse"],
                NumeroDeTelephone = (int)reader["NumeroTelephone"],
                Adresse = (string)reader["Adresse"],
                CodePostal = (int)reader["CodePostal"],
                Ville = (string)reader["Ville"],
                Gestionnaire = (bool)reader["Gestionnaire"]
            };
        }

        return new Utilisateur
        {
            Id = 0,
            Nom = nom,
            Prenom = prenom,
            Email = email,
            Login = login,
            MotDePasse = motDePasse,
            NumeroDeTelephone = numeroDeTelephone,
            Adresse = adresse,
            CodePostal = int.TryParse(codePostal, out var cp) ? cp : 0,
            Ville = ville,
            Gestionnaire = gestionnaire
        };
    }
    
    
    // Permet de lire la PS et de récupérer tous les utilisateurs de la base de données
    public List<Utilisateur> LireUtilisateurs()
    {
        var utilisateurs = new List<Utilisateur>();
        string query = "PS_LireUtilisateurs";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            utilisateurs.Add(new Utilisateur
            {
                Id = reader["IDUtilisateur"] is DBNull ? 0 : (int)reader["IDUtilisateur"],
                Nom = reader["Nom"] is DBNull ? string.Empty : (string) reader["Nom"],
                Prenom = reader["Prenom"] is DBNull ? string.Empty : (string) reader["Prenom"],
                Email = reader["Email"] is DBNull ? string.Empty : (string) reader["Email"],
                NumeroDeTelephone = reader["NumeroTelephone"] is DBNull ? 0 : (int) reader["NumeroTelephone"],
                Adresse = reader["Adresse"] is DBNull ? string.Empty : (string) reader["Adresse"],
                CodePostal = reader["CodePostal"] is DBNull ? 0 : (int) reader["CodePostal"],
                Ville = reader["Ville"] is DBNull ? string.Empty : (string) reader["Ville"],
                Gestionnaire = reader["Gestionnaire"] is DBNull ? false : (bool) reader["Gestionnaire"]
            });
        }

        return utilisateurs;
    }
    
    // Permet de lire la PS et de supprimer un utilisateurs de la base de données et de l'application
    public bool SupprimerUtilisateur(int idUtilisateur)
    {
        string query = "PS_SupprimerUtilisateur";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDUtilisateur", idUtilisateur);
        try
        {
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public string GetTransport(int IDTrajet) 
    {
        string query = "PS_LireTypeTransport";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdTrajet", IDTrajet);
        using var reader = command.ExecuteReader();
        reader.Read();
        return (string)reader["TypeTransport"];
    }

    public List<Trajet> GetVoyage(int IDUtilisateur)
    {
        string query = "PS_LireVoyage";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdUtilisateur", IDUtilisateur);
        using var reader = command.ExecuteReader();
        List<Trajet> data = new List<Trajet>();
        while (reader.Read())
        {
            DateTime dateDepart = (DateTime)reader["DateDepart"];
            TimeSpan heureDepart = (TimeSpan)reader["HeureDepart"];
            DateTime dateArrivee = (DateTime)reader["DateArrivee"];
            TimeSpan heureArrivee = (TimeSpan)reader["HeureArrivee"];
            
            Trajet trajet = new Trajet
            {
                IDTrajet = (int)reader["IDTrajet"],
                DateDepart = new DateTime(dateDepart.Year, dateDepart.Month, dateDepart.Day),
                HeureDepart = new TimeSpan( heureDepart.Hours, heureDepart.Minutes, heureDepart.Seconds),
                DateArrivee = new DateTime(dateArrivee.Year, dateArrivee.Month, dateArrivee.Day),
                HeureArrivee = new TimeSpan(heureArrivee.Hours, heureArrivee.Minutes, heureArrivee.Seconds),
                Statut = (string)reader["Statut"],
                VilleArrivee = (string)reader["VilleArrivee"],
                VilleDepart = (string)reader["VilleDepart"],
                TypeTransport = (string)reader["TypeTransport"]
            };
            
            data.Add(trajet);
        }
        return data;
    }
}