using Avalonia.Media;

namespace GC.Core;

public static class Styles {

  public static SolidColorBrush GetBackgroundBrush() => new(Hub.IsDarkMode ? Color.Parse("#000000") : Color.Parse("#ffffff"));

  public static SolidColorBrush GetTextBrush() => new(Hub.IsDarkMode ? Colors.White : Colors.Black);

  public static SolidColorBrush GetSecondaryTextBrush() => new(Color.Parse("#8E8E93"));
}