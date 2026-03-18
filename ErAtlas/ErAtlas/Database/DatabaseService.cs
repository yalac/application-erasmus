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

    public bool checkUser(string username, string password)
    {
        return true;
    }
}
