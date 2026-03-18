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

    public User GetUser(string username)
    {
        string query = "GetUsers";
        SqlCommand command = new SqlCommand(query, _connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("username", username);
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        return new User
        {
            Id = (int)reader["ID"],
            Username = (string)reader["Username"],
            Password = (string)reader["Password"]
        };
    }

    public bool checkUser(string username, string password)
    {
        try
        {
            User user = GetUser(username);
            return user.Password == password;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
