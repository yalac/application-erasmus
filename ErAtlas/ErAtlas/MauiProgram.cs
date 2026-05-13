using Microsoft.Extensions.Logging;
using ErAtlas.Database;
using ErAtlas.View;
using ErAtlas.ViewModels;
using ErAtlas.Views;
using System.Diagnostics;

namespace ErAtlas;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        RegisterGlobalExceptionHandlers();

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
        builder.Services.AddSingleton<MyTravelDetailsPage>();
        builder.Services.AddSingleton<MyTravelPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<TripManagementPage>();
        builder.Services.AddSingleton<UsersManagementPage>();
        
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<MyTravelDetailsViewModel>();
        builder.Services.AddSingleton<MyTravelViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<TripManagementViewModel>();
        builder.Services.AddSingleton<UsersManagementViewModel>();


        return builder.Build();
    }

    private static void RegisterGlobalExceptionHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            LogException(args.ExceptionObject as Exception ?? new Exception("Unhandled exception inconnue."));

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            LogException(args.Exception);
            args.SetObserved();
        };
    }

    private static void LogException(Exception ex)
    {
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logDir = Path.Combine(appData, "ErAtlas");
            Directory.CreateDirectory(logDir);

            var logPath = Path.Combine(logDir, "startup-crash.log");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}\n\n");
            Debug.WriteLine(ex.ToString());
        }
        catch
        {
        }
    }
}