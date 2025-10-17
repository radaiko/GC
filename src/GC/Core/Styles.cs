using System;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace GC.Core;

public static class Styles {
  
  // Brushes -------------------------------------------------------------------
  public static SolidColorBrush BackgroundBrush => new(Base.IsDarkMode ? Color.Parse("#000000") : Color.Parse("#ffffff"));
  public static SolidColorBrush TextBrush => new(Base.IsDarkMode ? Colors.White : Colors.Black);
  public static SolidColorBrush SecondaryTextBrush => new(Color.Parse("#8E8E93"));

  
  // Icons ---------------------------------------------------------------------
  public static Bitmap OrderIcon => new(GetIconStream("order"));
  public static Bitmap BillingIcon => new(GetIconStream("billing"));
  public static Bitmap SettingsIcon => new(GetIconStream("settings"));
  
  

  
  // Private methods -----------------------------------------------------------
  private static Stream GetIconStream(string baseName) => AssetLoader.Open(new Uri(GetIconUri(baseName)));

  private static string GetIconUri(string baseName) {
    var suffix = Base.IsDarkMode ? "dark" : "light";
    return $"avares://GC/Assets/Icons/{baseName}-{suffix}.png";
  }
}