using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Task5.Common;
using Task5.ViewModels;
using Task5.Views;

namespace Task5;

public partial class App : Application
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ConfigureDependencyInjection();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Languages.Resources.Culture = new CultureInfo("ru-RU");
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = ServiceProvider.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = ServiceProvider.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private static void ConfigureDependencyInjection()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        ServiceProvider = collection.BuildServiceProvider();
    }
}
