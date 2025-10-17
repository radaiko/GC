using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GC.ViewModels;

public partial class MainViewModel : ObservableObject {
  public string Greeting => "Welcome to Avalonia!";
  
  [ObservableProperty] private string _mobileTest = "This is a mobile test string.";


  [RelayCommand]
  private static void SwitchOrder() {
    // Logic to switch to order view
  }
  
  [RelayCommand]
  private static void SwitchToBilling() {
    // Logic to switch to billing view
  }
  
  [RelayCommand]
  private static void SwitchToSettings() {
    // Logic to switch to settings view
  }
}