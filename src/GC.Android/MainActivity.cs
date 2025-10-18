using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using GC.Core;

namespace GC.Android;

[Activity(
  Label = "GC.Android",
  Theme = "@style/MyTheme.NoActionBar",
  Icon = "@drawable/icon",
  MainLauncher = true,
  ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App> {
  protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
    Initialize();
    return base.CustomizeAppBuilder(builder)
      .WithInterFont()
      .UseReactiveUI();
  }

  void Initialize() {
    Base.DeviceKey = DeviceKeyDeriver.GetDevicePassphrase();
  }
}