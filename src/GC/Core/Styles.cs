using System;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace GC.Core;

public static class Styles {
  
  // Brushes -------------------------------------------------------------------
  public static SolidColorBrush BackgroundBrush => new(Base.IsDarkMode ? Color.Parse("#000000") : Color.Parse("#ffffff"));
  public static SolidColorBrush AccentBrush =>  new(Base.IsDarkMode ? Color.Parse("#1C1C1E") : Color.Parse("#1C1C1F"));
  public static SolidColorBrush TextBrush => new(Base.IsDarkMode ? Colors.White : Colors.Black);
  public static SolidColorBrush SecondaryTextBrush => new(Color.Parse("#8E8E93"));

  // Margins -------------------------------------------------------------------
  public static readonly Thickness StandardPadding = new(StandardMargin);
  public static readonly Thickness CompactPadding = new(CompactMargin);
  public static readonly Thickness LargePadding = new(LargeMargin);

  private const double StandardMargin = 16.0;
  private const double CompactMargin = 8.0;
  private const double LargeMargin = 24.0;
  
  // Radius --------------------------------------------------------------------
  public static readonly CornerRadius StandardCornerRadius = new CornerRadius(StandardRadius);
  public static readonly  CornerRadius LargeCornerRadius = new CornerRadius(LargeRadius);
  
  private const double StandardRadius = 8.0;
  private const double LargeRadius = 16.0;
  
  
  // Icons ---------------------------------------------------------------------
  // Lazily created on the UI thread to prevent native-platform image/texture
  // creation from occurring on background threads (which causes iOS layer
  // modification crashes).
  private static readonly object _iconLock = new();
  private static Bitmap? _orderIcon;
  private static Bitmap? _billingIcon;
  private static Bitmap? _settingsIcon;

  public static Bitmap OrderIcon => EnsureIcon(ref _orderIcon, "order");
  public static Bitmap BillingIcon => EnsureIcon(ref _billingIcon, "billing");
  public static Bitmap SettingsIcon => EnsureIcon(ref _settingsIcon, "settings");
  
  
  
  // Private methods -----------------------------------------------------------
  private static Stream GetIconStream(string baseName) => AssetLoader.Open(new Uri(GetIconUri(baseName)));

  private static string GetIconUri(string baseName) {
    var suffix = Base.IsDarkMode ? "dark" : "light";
    return $"avares://GC/Assets/Icons/{baseName}-{suffix}.png";
  }

  private static Bitmap EnsureIcon(ref Bitmap? field, string baseName) {
    if (field is not null) {
      Log.Info($"Icon '{baseName}' already initialized (fast path)");
      return field;
    }
  
    lock (_iconLock) {
      if (field is not null) {
        Log.Info($"Icon '{baseName}' already initialized (lock path)");
        return field;
      }
  
      Log.Info($"Creating icon '{baseName}' (dark={Base.IsDarkMode})");
  
      // If we're already on the UI thread, create directly.
      if (Dispatcher.UIThread.CheckAccess()) {
        Log.Info($"On UI thread - creating Bitmap for '{baseName}'");
        field = new Bitmap(GetIconStream(baseName));
        Log.Info($"Created Bitmap for '{baseName}'");
        return field;
      }
  
      // Otherwise, marshal creation to the UI thread and wait for the result.
      // Creating platform Bitmaps may touch native layers/textures which must
      // happen on the main thread on iOS.
      Log.Info($"Off UI thread - invoking UI thread to create Bitmap for '{baseName}'");
      var task = Dispatcher.UIThread.InvokeAsync(() => {
        Log.Info($"UI thread: creating Bitmap for '{baseName}'");
        return new Bitmap(GetIconStream(baseName));
      });
      field = task.GetAwaiter().GetResult();
      Log.Info($"Created Bitmap for '{baseName}' via UI thread");
      return field;
    }
  }

  // Public helper to pre-warm all icons on the UI thread. Call this from
  // framework initialization to ensure platform resources are created safely.
  public static void PreWarmIcons() {
    Log.Info("Pre-warming platform icons");
    _ = OrderIcon;
    Log.Info("Pre-warmed OrderIcon");
    _ = BillingIcon;
    Log.Info("Pre-warmed BillingIcon");
    _ = SettingsIcon;
    Log.Info("Pre-warmed SettingsIcon");
  }
}