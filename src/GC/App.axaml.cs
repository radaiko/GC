using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GC.Core;
using GC.ViewModels;
using GC.Views;

namespace GC;

public partial class App : Application {
  public override void Initialize() {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted() {
    Log.Info("Application framework initialization completed");
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
      Log.Info("Initializing MainWindow for desktop application");
      desktop.MainWindow = new MainWindow {
        DataContext = new MainViewModel()
      };
    }
    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
      Log.Info("Initializing MainView for single view application");
      singleViewPlatform.MainView = new MainView {
        DataContext = new MainViewModel()
      };
    }

    base.OnFrameworkInitializationCompleted();
  }
}