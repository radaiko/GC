using CommunityToolkit.Mvvm.ComponentModel;
using GC.Core;

namespace GC.ViewModels;

public partial class SettingsViewModel : ObservableObject {
  [ObservableProperty] private string? _gourmetUsername;
  [ObservableProperty] private string? _gourmetPassword;
  [ObservableProperty] private string? _ventoUsername;
  [ObservableProperty] private string? _ventoPassword;
  [ObservableProperty] private bool _debugModeEnabled;
  
  public SettingsViewModel() {
    var settings = Hub.Settings;
    _gourmetUsername = settings.GourmetUsername;
    _gourmetPassword = settings.GourmetPassword;
    _ventoUsername = settings.VentoUsername;
    _ventoPassword = settings.VentoPassword;
    _debugModeEnabled = settings.DebugMode;
  }
  
  
}