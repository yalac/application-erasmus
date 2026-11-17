using ErAtlas.ViewModels;

namespace ErAtlas.View;

public partial class LoginPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /* Méthode permettant de passer au mot de passe lorsque l'on appuie sur "entrée" dans Username */
    private void OnUsernameCompleted(object? sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }

    /* Méthode permettant de se connecter lorsque l'on appuie sur "entrée" dans Mot de passe */
    private void OnPasswordCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is LoginViewModel viewModel)
        {
            viewModel.LoginCommand.Execute(null);
        }
    }
}