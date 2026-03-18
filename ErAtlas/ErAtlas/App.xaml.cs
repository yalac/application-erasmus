using ErAtlas.Database;

namespace ErAtlas;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new LoginPage(new DatabaseService()));
    }
}
