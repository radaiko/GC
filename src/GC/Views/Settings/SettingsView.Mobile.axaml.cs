using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace GC.Views.Settings;

public partial class SettingsViewMobile : UserControl {
  public SettingsViewMobile() {
    InitializeComponent();
    DataContext = new GC.ViewModels.SettingsViewModel();
  }
  private void Button_OnClick(object? sender, RoutedEventArgs e) {
    Debugger.Break();
  }
}
