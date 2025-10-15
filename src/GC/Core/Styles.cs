using Avalonia.Media;

namespace GC.Core;

public static class Styles {

  public static SolidColorBrush GetBackgroundBrush() => new(Base.IsDarkMode ? Color.Parse("#000000") : Color.Parse("#ffffff"));

  public static SolidColorBrush GetTextBrush() => new(Base.IsDarkMode ? Colors.White : Colors.Black);

  public static SolidColorBrush GetSecondaryTextBrush() => new(Color.Parse("#8E8E93"));
}