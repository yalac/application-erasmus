using System.Security.Cryptography;
using System.Text;
using System.Data;
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
    }

    private void EnsureConnectionOpen()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    // Permet de lire la PS de vérification de la connexion et de renvoyé l'id, le mdp et le login si elle est vrai
    public Utilisateur? VerifUser(string login)
    {
        EnsureConnectionOpen();
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
        EnsureConnectionOpen();
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
    
    // Permet de lire la PS et de modifier un utilisateurs de la base de données et de l'application
    public Utilisateur ModifierUtilisateur(int idUtilisateur, string nom, string prenom, string email, string login, string motDePasse, int numeroDeTelephone, string adresse, string codePostal, string ville, bool gestionnaire)
    {
        string query = "PS_ModifierUtilisateur";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDUtilisateur", idUtilisateur);
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
            Id = idUtilisateur,
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

    // =========================
    // LIEUX
    // =========================

    public List<Lieu> LireLieux()
    {
        var lieux = new List<Lieu>();
        string query = "PS_LireLieux";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            lieux.Add(new Lieu
            {
                IdLieu = reader["IDLieu"] is DBNull ? 0 : (int)reader["IDLieu"],
                Nom = reader["Nom"] is DBNull ? string.Empty : (string)reader["Nom"],
                Adresse = reader["Adresse"] is DBNull ? string.Empty : (string)reader["Adresse"],
                CodePostal = reader["CodePostal"] is DBNull ? 0 : (int)reader["CodePostal"],
                Ville = reader["Ville"] is DBNull ? string.Empty : (string)reader["Ville"],
                Pays = reader["Pays"] is DBNull ? string.Empty : (string)reader["Pays"]
            });
        }

        return lieux;
    }

    public Lieu CreationLieu(string nom, string adresse, string codePostal, string ville, string pays)
    {
        string query = "PS_CreationLieu";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Nom", nom);
        command.Parameters.AddWithValue("@Adresse", adresse);
        command.Parameters.AddWithValue("@CodePostal", codePostal);
        command.Parameters.AddWithValue("@Ville", ville);
        command.Parameters.AddWithValue("@Pays", pays);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Lieu
            {
                IdLieu = reader["IDLieu"] is DBNull ? 0 : (int)reader["IDLieu"],
                Nom = reader["Nom"] is DBNull ? nom : (string)reader["Nom"],
                Adresse = reader["Adresse"] is DBNull ? adresse : (string)reader["Adresse"],
                CodePostal = reader["CodePostal"] is DBNull ? int.TryParse(codePostal, out var cp) ? cp : 0 : (int)reader["CodePostal"],
                Ville = reader["Ville"] is DBNull ? ville : (string)reader["Ville"],
                Pays = reader["Pays"] is DBNull ? pays : (string)reader["Pays"]
            };
        }

        return new Lieu
        {
            IdLieu = 0,
            Nom = nom,
            Adresse = adresse,
            CodePostal = int.TryParse(codePostal, out var codePostalNumerique) ? codePostalNumerique : 0,
            Ville = ville,
            Pays = pays
        };
    }

    public Lieu ModifierLieu(int idLieu, string nom, string adresse, string codePostal, string ville, string pays)
    {
        string query = "PS_ModifierLieu";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDLieu", idLieu);
        command.Parameters.AddWithValue("@Nom", nom);
        command.Parameters.AddWithValue("@Adresse", adresse);
        command.Parameters.AddWithValue("@CodePostal", codePostal);
        command.Parameters.AddWithValue("@Ville", ville);
        command.Parameters.AddWithValue("@Pays", pays);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Lieu
            {
                IdLieu = reader["IDLieu"] is DBNull ? idLieu : (int)reader["IDLieu"],
                Nom = reader["Nom"] is DBNull ? nom : (string)reader["Nom"],
                Adresse = reader["Adresse"] is DBNull ? adresse : (string)reader["Adresse"],
                CodePostal = reader["CodePostal"] is DBNull ? int.TryParse(codePostal, out var cp) ? cp : 0 : (int)reader["CodePostal"],
                Ville = reader["Ville"] is DBNull ? ville : (string)reader["Ville"],
                Pays = reader["Pays"] is DBNull ? pays : (string)reader["Pays"]
            };
        }

        return new Lieu
        {
            IdLieu = idLieu,
            Nom = nom,
            Adresse = adresse,
            CodePostal = int.TryParse(codePostal, out var codePostalNumerique) ? codePostalNumerique : 0,
            Ville = ville,
            Pays = pays
        };
    }

    public bool SupprimerLieu(int idLieu)
    {
        string query = "PS_SupprimerLieu";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDLieu", idLieu);

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

    // =========================
    // TRANSPORTS
    // =========================

    public List<Transport> LireTransports()
    {
        var transports = new List<Transport>();
        string query = "PS_LireTransports";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            transports.Add(new Transport
            {
                IdTransport = reader["IDTransport"] is DBNull ? 0 : (int)reader["IDTransport"],
                TypeTransport = reader["TypeTransport"] is DBNull ? string.Empty : (string)reader["TypeTransport"],
                Capacite = reader["Capacite"] is DBNull ? 0 : (int)reader["Capacite"],
                Immatriculation = reader["Immatriculation"] is DBNull ? string.Empty : (string)reader["Immatriculation"],
                Description = reader["Description"] is DBNull ? string.Empty : (string)reader["Description"]
            });
        }

        return transports;
    }

    public Transport CreationTransport(string typeTransport, int capacite, string immatriculation, string description)
    {
        string query = "PS_CreationTransport";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TypeTransport", typeTransport);
        command.Parameters.AddWithValue("@Capacite", capacite);
        command.Parameters.AddWithValue("@Immatriculation", immatriculation);
        command.Parameters.AddWithValue("@Description", description);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Transport
            {
                IdTransport = reader["IDTransport"] is DBNull ? 0 : (int)reader["IDTransport"],
                TypeTransport = reader["TypeTransport"] is DBNull ? typeTransport : (string)reader["TypeTransport"],
                Capacite = reader["Capacite"] is DBNull ? capacite : (int)reader["Capacite"],
                Immatriculation = reader["Immatriculation"] is DBNull ? immatriculation : (string)reader["Immatriculation"],
                Description = reader["Description"] is DBNull ? description : (string)reader["Description"]
            };
        }

        return new Transport
        {
            IdTransport = 0,
            TypeTransport = typeTransport,
            Capacite = capacite,
            Immatriculation = immatriculation,
            Description = description
        };
    }

    public Transport ModifierTransport(int idTransport, string typeTransport, int capacite, string immatriculation, string description)
    {
        string query = "PS_ModifierTransport";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDTransport", idTransport);
        command.Parameters.AddWithValue("@TypeTransport", typeTransport);
        command.Parameters.AddWithValue("@Capacite", capacite);
        command.Parameters.AddWithValue("@Immatriculation", immatriculation);
        command.Parameters.AddWithValue("@Description", description);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Transport
            {
                IdTransport = reader["IDTransport"] is DBNull ? idTransport : (int)reader["IDTransport"],
                TypeTransport = reader["TypeTransport"] is DBNull ? typeTransport : (string)reader["TypeTransport"],
                Capacite = reader["Capacite"] is DBNull ? capacite : (int)reader["Capacite"],
                Immatriculation = reader["Immatriculation"] is DBNull ? immatriculation : (string)reader["Immatriculation"],
                Description = reader["Description"] is DBNull ? description : (string)reader["Description"]
            };
        }

        return new Transport
        {
            IdTransport = idTransport,
            TypeTransport = typeTransport,
            Capacite = capacite,
            Immatriculation = immatriculation,
            Description = description
        };
    }

    public bool SupprimerTransport(int idTransport)
    {
        string query = "PS_SupprimerTransport";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDTransport", idTransport);

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
    
    
    // =========================
    // Trajets
    // =========================

    public List<Trajet> LireTrajets()
    {
        var trajets = new List<Trajet>();
        string query = "PS_LireTrajets";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            trajets.Add(new Trajet
            {
                IDTrajet = reader["IDTrajet"] is DBNull ? 0 : (int)reader["IDTrajet"],
                DateDepart = reader["DateDepart"] is DBNull ? DateTime.MinValue : (DateTime)reader["DateDepart"],
                HeureDepart = reader["HeureDepart"] is DBNull ? TimeSpan.Zero : (TimeSpan)reader["HeureDepart"],
                DateArrivee = reader["DateArrivee"] is DBNull ? DateTime.MinValue : (DateTime)reader["DateArrivee"],
                HeureArrivee = reader["HeureArrivee"] is DBNull ? TimeSpan.Zero : (TimeSpan)reader["HeureArrivee"],
                Statut = reader["Statut"] is DBNull ? string.Empty : (string)reader["Statut"],
                IDLieuArrivee = reader["IDLieuArrivee"] is DBNull ? 0 : (int)reader["IDLieuArrivee"],
                IDLieuDepart = reader["IDLieuDepart"] is DBNull ? 0 : (int)reader["IDLieuDepart"],
                IDTransport = reader["IDTransport"] is DBNull ? 0 : (int)reader["IDTransport"]
            });
        }
        return trajets;
    }

    public Trajet CreationTrajet(DateTime dateDepart, TimeSpan heureDepart, DateTime dateArrivee, TimeSpan heureArrivee, string statut, int idLieuArrivee, int idLieuDepart, int idTransport)
    {
        string query = "PS_CreationTrajet";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@DateDepart", dateDepart);
        command.Parameters.AddWithValue("@HeureDepart", heureDepart);
        command.Parameters.AddWithValue("@DateArrivee", dateArrivee);
        command.Parameters.AddWithValue("@HeureArrivee", heureArrivee);
        command.Parameters.AddWithValue("@Statut", statut);
        command.Parameters.AddWithValue("@IDLieuArrivee", idLieuArrivee);
        command.Parameters.AddWithValue("@IDLieuDepart", idLieuDepart);
        command.Parameters.AddWithValue("@IDTransport", idTransport);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Trajet
            {
                IDTrajet = (int)reader["IDTrajet"],
                DateDepart = (DateTime)reader["DateDepart"],
                HeureDepart = (TimeSpan)reader["HeureDepart"],
                DateArrivee = (DateTime)reader["DateArrivee"],
                HeureArrivee = (TimeSpan)reader["HeureArrivee"],
                Statut = (string)reader["Statut"],
                IDLieuArrivee = (int)reader["IDLieuArrivee"],
                IDLieuDepart = (int)reader["IDLieuDepart"],
                IDTransport = (int)reader["IDTransport"]
            };
        }

        return new Trajet
        {
            IDTrajet = 0,
            DateDepart = dateDepart,
            HeureDepart = heureDepart,
            DateArrivee = dateArrivee,
            HeureArrivee = heureArrivee,
            Statut = statut,
            IDLieuArrivee = idLieuArrivee,
            IDLieuDepart = idLieuDepart,
            IDTransport = idTransport
        };
    }

    public Trajet ModifierTrajet(int idTrajet, DateTime dateDepart, TimeSpan heureDepart, DateTime dateArrivee, TimeSpan heureArrivee, string statut, int idLieuArrivee, int idLieuDepart, int idTransport)
    {
        string query = "PS_ModifierTrajet";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@IDTrajet", idTrajet);
        command.Parameters.AddWithValue("@DateDepart", dateDepart);
        command.Parameters.AddWithValue("@HeureDepart", heureDepart);
        command.Parameters.AddWithValue("@DateArrivee", dateArrivee);
        command.Parameters.AddWithValue("@HeureArrivee", heureArrivee);
        command.Parameters.AddWithValue("@Statut", statut);
        command.Parameters.AddWithValue("@IDLieuArrivee", idLieuArrivee);
        command.Parameters.AddWithValue("@IDLieuDepart", idLieuDepart);
        command.Parameters.AddWithValue("@IDTransport", idTransport);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Trajet
            {
                IDTrajet = (int)reader["IDTrajet"],
                DateDepart = (DateTime)reader["DateDepart"],
                HeureDepart = (TimeSpan)reader["HeureDepart"],
                DateArrivee = (DateTime)reader["DateArrivee"],
                HeureArrivee = (TimeSpan)reader["HeureArrivee"],
                Statut = (string)reader["Statut"],
                IDLieuArrivee = (int)reader["IDLieuArrivee"],
                IDLieuDepart = (int)reader["IDLieuDepart"],
                IDTransport = (int)reader["IDTransport"]
            };
        }

        return new Trajet
        {
            IDTrajet = idTrajet,
            DateDepart = dateDepart,
            HeureDepart = heureDepart,
            DateArrivee = dateArrivee,
            HeureArrivee = heureArrivee,
            Statut = statut,
            IDLieuArrivee = idLieuArrivee,
            IDLieuDepart = idLieuDepart,
            IDTransport = idTransport
        };
    }

    public bool SupprimerTrajet(int idTrajet)
    {
        string query = "PS_SupprimerTrajet";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDTrajet", idTrajet);

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
    
    // Surcharge pour modifier un utilisateur sans changer le mot de passe
    public Utilisateur ModifierUtilisateur(int idUtilisateur, string nom, string prenom, string email, string login, int numeroDeTelephone, string adresse, string codePostal, string ville, bool gestionnaire)
    {
        string query = "PS_ModifierUtilisateur";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IDUtilisateur", idUtilisateur);
        command.Parameters.AddWithValue("@Nom", nom);
        command.Parameters.AddWithValue("@Prenom", prenom);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@Login", login);
        command.Parameters.AddWithValue("@MotDePasse", DBNull.Value);
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
            Id = idUtilisateur,
            Nom = nom,
            Prenom = prenom,
            Email = email,
            Login = login,
            NumeroDeTelephone = numeroDeTelephone,
            Adresse = adresse,
            CodePostal = int.TryParse(codePostal, out var cp) ? cp : 0,
            Ville = ville,
            Gestionnaire = gestionnaire
        };
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
        static bool HasColumn(SqlDataReader r, string columnName)
        {
            try
            {
                return r.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        string query = "PS_LireVoyage";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdUtilisateur", IDUtilisateur);
        using var reader = command.ExecuteReader();
        List<Trajet> data = new List<Trajet>();
        while (reader.Read())
        {
            int idTrajet = HasColumn(reader, "IDTrajet") && reader["IDTrajet"] != DBNull.Value
                ? (int)reader["IDTrajet"]
                : 0;

            DateTime dateDepart = HasColumn(reader, "DateDepart") && reader["DateDepart"] != DBNull.Value
                ? (DateTime)reader["DateDepart"]
                : DateTime.MinValue;

            TimeSpan heureDepart = HasColumn(reader, "HeureDepart") && reader["HeureDepart"] != DBNull.Value
                ? (TimeSpan)reader["HeureDepart"]
                : TimeSpan.Zero;

            DateTime dateArrivee = HasColumn(reader, "DateArrivee") && reader["DateArrivee"] != DBNull.Value
                ? (DateTime)reader["DateArrivee"]
                : DateTime.MinValue;

            TimeSpan heureArrivee = HasColumn(reader, "HeureArrivee") && reader["HeureArrivee"] != DBNull.Value
                ? (TimeSpan)reader["HeureArrivee"]
                : TimeSpan.Zero;
            
            DateTime fullDateDepart = dateDepart.Date + heureDepart;
            DateTime fullDateArrivee = dateArrivee.Date + heureArrivee;
            
            Trajet trajet = new Trajet
            {
                IDTrajet = idTrajet,
                DateDepart = fullDateDepart,
                HeureDepart = heureDepart,    
                DateArrivee = fullDateArrivee, 
                HeureArrivee = heureArrivee,  
                Statut = HasColumn(reader, "Statut") && reader["Statut"] != DBNull.Value
                    ? (string)reader["Statut"]
                    : string.Empty,
                VilleArrivee = HasColumn(reader, "VilleArrivee") && reader["VilleArrivee"] != DBNull.Value
                    ? (string)reader["VilleArrivee"]
                    : string.Empty,
                VilleDepart = HasColumn(reader, "VilleDepart") && reader["VilleDepart"] != DBNull.Value
                    ? (string)reader["VilleDepart"]
                    : string.Empty,
                TypeTransport = HasColumn(reader, "TypeTransport") && reader["TypeTransport"] != DBNull.Value
                    ? (string)reader["TypeTransport"]
                    : string.Empty
            };
            
            // Récupérer les IDs uniquement si les colonnes existent dans le result set
            if (HasColumn(reader, "IDLieuDepart") && reader["IDLieuDepart"] != DBNull.Value)
                trajet.IDLieuDepart = (int)reader["IDLieuDepart"];
            if (HasColumn(reader, "IDLieuArrivee") && reader["IDLieuArrivee"] != DBNull.Value)
                trajet.IDLieuArrivee = (int)reader["IDLieuArrivee"];
            if (HasColumn(reader, "IDTransport") && reader["IDTransport"] != DBNull.Value)
                trajet.IDTransport = (int)reader["IDTransport"];
            
            data.Add(trajet);
        }
        return data;
    }
}