using CommunityToolkit.Mvvm.ComponentModel;
using GC.Core;
using GC.Models;

namespace GC.ViewModels;

public partial class SettingsViewModel : ObservableObject {
  public Settings Settings { get; } = Base.Settings;
}