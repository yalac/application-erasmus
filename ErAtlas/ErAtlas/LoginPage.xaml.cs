namespace ErAtlas;
using Database;
public partial class LoginPage
{
    private readonly DatabaseService _databaseService;
    public LoginPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text.Trim();
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Veuillez remplir tous les champs.";
            ErrorLabel.IsVisible = true;
            return;
        }
        
        bool isValidUser = _databaseService.checkUser(username, password);
        
        if (isValidUser)
        {
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            ErrorLabel.Text = "Nom d'utilisateur ou mot de passe incorrect.";
            ErrorLabel.IsVisible = true;
        }
        
    }
}