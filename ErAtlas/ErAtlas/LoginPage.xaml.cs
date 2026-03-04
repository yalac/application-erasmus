namespace ErAtlas;

public partial class LoginPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Recuperer les valeurs saisies.
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Veuillez remplir tous les champs.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (ValidateLogin(username, password))
        {
            ErrorLabel.IsVisible = false;
            await Shell.Current.GoToAsync("//main");
        }
        else
        {
            ErrorLabel.Text = "Nom d'utilisateur ou mot de passe incorrect.";
            ErrorLabel.IsVisible = true;
            PasswordEntry.Text = string.Empty;
        }
    }

    private bool ValidateLogin(string username, string password)
    {
        return !string.IsNullOrEmpty(username) && password == "password";
    }
}