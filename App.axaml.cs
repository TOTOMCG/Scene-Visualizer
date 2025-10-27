using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace SceneVisualizer;

/// <summary>
/// Главный класс приложения Avalonia
/// </summary>
/// <remarks>
/// Отвечает за инициализацию приложения и создание главного окна
/// </remarks>
public class App : Application
{
    /// <summary>
    /// Инициализация Avalonia XAML
    /// </summary>
    public override void Initialize()
    { 
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Создание главного окна после инициализации фреймворка
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    { 
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            
        } 
        base.OnFrameworkInitializationCompleted();
    }
}
