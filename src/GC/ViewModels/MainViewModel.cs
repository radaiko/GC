using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GC.Core;
using GC.Views.Order;
using GC.Views.Settings;

namespace GC.ViewModels;

public partial class MainViewModel : ObservableObject {
  public string Greeting => "Welcome to Avalonia!";

  [ObservableProperty] private string _mobileTest = "This is a mobile test string.";
  [ObservableProperty] private object? _currentView;

  public MainViewModel() {
    // Try to load settings to see if we are already setup
    var settings = Base.Settings;
    if (settings.GourmetUsername == null || settings.VentoUsername == null ||
        settings.GourmetPassword == null || settings.VentoPassword == null) {
      Log.Info("Settings incomplete, switching to settings view");
      // Switch to settings view
      SwitchToSettings();
    } else {
      Log.Info("Settings complete, switching to order view");
      // Switch to order view
      SwitchOrder();
    }
  }

  [RelayCommand]
  private void SwitchOrder() {
    Log.Info("Switching to order view");
    if (Base.IsMobile) {
      CurrentView = new OrderViewMobile();
    }
    else {
      Log.Error ("OrderViewDesktop not implemented yet");
    }
  }

  [RelayCommand]
  private void SwitchToBilling() {
    Log.Info("Switching to billing view");
    // Logic to switch to billing view
  }

  [RelayCommand]
  private void SwitchToSettings() {
    Log.Info("Switching to settings view");
    if (Base.IsMobile) {
      // Switch to SettingsViewMobile
      CurrentView = new SettingsViewMobile();
    }
    else {
      Log.Error ("SettingsViewDesktop not implemented yet");
    }
  }
}