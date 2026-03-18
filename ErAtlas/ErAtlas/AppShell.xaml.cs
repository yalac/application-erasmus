using ErAtlas.View;

namespace ErAtlas;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute(nameof(MyTravelPage), typeof(MyTravelPage));
        Routing.RegisterRoute(nameof(TripManagementPage), typeof(TripManagementPage));
        Routing.RegisterRoute(nameof(UsersManagementPage), typeof(UsersManagementPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }
}