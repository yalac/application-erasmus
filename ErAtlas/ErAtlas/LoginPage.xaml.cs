namespace ErAtlas;

public partial class LoginPage
{
    public LoginPage()
    {
        InitializeComponent();
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

        if (User.ValidateCredentials(username, password))
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
}