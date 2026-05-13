using ErAtlas.View;

namespace ErAtlas;

public partial class App : Application
{
    private readonly IServiceProvider _services;
    private string? _startupError;

    public App(IServiceProvider services)
    {
        _services = services;

        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            _startupError = ex.ToString();
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (_startupError is not null)
        {
            return new Window(CreateErrorPage(_startupError));
        }

        try
        {
            var loginPage = _services.GetService(typeof(LoginPage)) as Page;
            return new Window(loginPage ?? CreateErrorPage("LoginPage introuvable dans le conteneur de services."));
        }
        catch (Exception ex)
        {
            return new Window(CreateErrorPage(ex.ToString()));
        }
    }

    private static ContentPage CreateErrorPage(string errorMessage)
    {
        return new ContentPage
        {
            Title = "Erreur de démarrage",
            BackgroundColor = Colors.White,
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 24,
                    Spacing = 12,
                    Children =
                    {
                        new Label
                        {
                            Text = "L'application n'a pas pu se lancer.",
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 18,
                            TextColor = Colors.Black
                        },
                        new Label
                        {
                            Text = errorMessage,
                            FontSize = 12,
                            TextColor = Colors.Black
                        }
                    }
                }
            }
        };
    }
}
