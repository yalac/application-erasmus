using Microsoft.Extensions.Logging;
using ErAtlas.Database;
using ErAtlas.View;
using ErAtlas.ViewModels;
using ErAtlas.Views;

namespace ErAtlas;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<AppShell>();
        
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MyTravelDescriptionPage>();
        builder.Services.AddSingleton<MyTravelPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<TripManagementPage>();
        builder.Services.AddSingleton<UsersManagementPage>();
        
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<MyTravelDescriptionViewModel>();
        builder.Services.AddSingleton<MyTravelViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<TripManagementViewModel>();
        builder.Services.AddSingleton<TravelViewModel>();
        builder.Services.AddSingleton<UsersManagementViewModel>();


        return builder.Build();
    }
}