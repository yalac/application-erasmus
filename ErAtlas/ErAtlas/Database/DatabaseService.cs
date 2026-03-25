using System.Security.Cryptography;
using System.Text;
using ErAtlas.Model;
using Microsoft.Data.SqlClient;

namespace ErAtlas.Database;

public class DatabaseService
{
    private SqlConnection _connection;
    private const string ConnectionString = "Server=localhost;Database=ErAtlas;User ID=sa;Password=Info76240#;TrustServerCertificate=True;";

    public DatabaseService()
    {
        _connection = new SqlConnection(ConnectionString);
        _connection.Open();
    }

    public User? GetUser(string username)
    {
        string query = "PS_VerificationConnexion";
        using var command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@username", username);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return null;
        return new User
        {
            Id = (int)reader["IDUtilisateur"],
            Username = (string)reader["Login"],
            Password = (string)reader["MotDePasse"]
        };
    }

    public bool CheckUser(string username, string password)
    {
        try
        {
            User? user = GetUser(username);
            if (user is null)
                return false;

            byte[] dataToHash = Encoding.UTF8.GetBytes(password);
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hashBytes = sha256Hash.ComputeHash(dataToHash);
                //string hashString = Encoding.UTF8.GetString(hashBytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                Console.WriteLine($"The SHA256 hash is: {hashString}");
                string dbHash = user.Password.Trim().Replace("-", "");
                return string.Equals(dbHash, hashString, StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}