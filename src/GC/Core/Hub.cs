using GC.Models;

namespace GC.Core;

public static class Hub {
  internal static bool IsTesting { get; set; } = false;
  public static bool IsDarkMode { get; set; } = false;
  public static Settings Settings { get; set; } = Settings.Load();
}